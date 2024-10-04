using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services.Application;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;

namespace TimeTrackerBlazorWitkac.Service.Application;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUsersService _usersService;
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<RoleEntity> _roleManager;
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IRolesRepository _rolesRepository;
    private readonly IEmailSender<UserEntity> _emailSender;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<AuthorizationService> _authorizationLogger;
    private IConfirmationTokensRepository _confirmationTokensRepository;

    public AuthorizationService(
        IUsersService usersService, 
        UserManager<UserEntity> userManager, 
        RoleManager<RoleEntity> roleManager,
        SignInManager<UserEntity> signInManager,
        IEmailSender<UserEntity> emailSender,
        NavigationManager navigationManager,
        IHttpContextAccessor contextAccessor,
        ITransactionRepository transactionRepository,
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IConfirmationTokensRepository confirmationTokensRepository,
        ILogger<AuthorizationService> authorizationLogger)
    {
        _usersService = usersService;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _navigationManager = navigationManager;
        _contextAccessor = contextAccessor;
        _transactionRepository = transactionRepository;
        _usersRepository = usersRepository;
        _rolesRepository = rolesRepository;
        _confirmationTokensRepository = confirmationTokensRepository;
        _authorizationLogger = authorizationLogger;
    }

    #region Registration

    //Need to take refreshToken from Microsoft and use it in Cookies, and take more info from MsAuthorization
    public async Task<Result<UserEntity>> RegisterWithExternalTokenAsync(User user, string msId, string authProvider, string roleStr)
    {
        var userExist = await _usersService.GetByEmailAsync<UserEntity>(user.Email);

        if (userExist.IsFailure)
        {
            return Result.Failure<UserEntity>(userExist.Error);
        }
        
        //TODO need to add refreshToken from Microsoft to cookies and make more checks for data from Ms (for example, check a specific MsId)
        if (userExist.Value != null)
        {
            var existingClaims = await _userManager.GetClaimsAsync(userExist.Value);
            var userMsId = existingClaims.FirstOrDefault(c => c.Type == "MsUserId");

            if (userMsId == null)
            {
                return Result.Failure<UserEntity>(AuthorizationServiceErrorMessages.UserNotFoundError.GetDescription());
            }

            if (userMsId.Value != msId)
            {
                return Result.Failure<UserEntity>(AuthorizationServiceErrorMessages.UserNotFoundError.GetDescription());
            }
            
            await SignInAsync(userExist.Value, true);
            return Result.Success(userExist.Value);
        }

        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var userCreationResult = await _usersRepository.CreateWithMsTokenAsync(user);
            if (userCreationResult.IsFailure)
            {
                throw new Exception(userCreationResult.Error);
            }

            //TODO add something else maybe userId from Microsoft to identify
            await _userManager.AddClaimAsync(userCreationResult.Value, new Claim("MsUserId", msId));
            await _userManager.AddClaimAsync(userCreationResult.Value, new Claim("AuthorizationType", authProvider));

            var role = await _roleManager.FindByNameAsync(roleStr);
            if (role == null)
            {
                var roleResult = Role.Builder()
                    .SetName(roleStr)
                    .Build();

                if (roleResult.IsFailure)
                {
                    throw new Exception(roleResult.Error);
                }
                
                var roleCreationResult = await _rolesRepository.CreateAsync<string>(roleResult.Value);
                if (roleCreationResult.IsFailure)
                {
                    throw new Exception(roleCreationResult.Error);
                }
            }

            
            var addUserToRoleResult = await _userManager.AddToRoleAsync(userCreationResult.Value, roleStr);
            if (!addUserToRoleResult.Succeeded)
            {
                throw new Exception(addUserToRoleResult.Errors.ToString());
            }

            await _transactionRepository.CommitTransactionAsync(transaction);
            await SignInAsync(userCreationResult.Value, true);

            return Result.Success(userCreationResult.Value);
        }
        catch (Exception e)
        {
            _authorizationLogger.LogError(e.Message);
            await _transactionRepository.RollbackTransactionAsync(transaction);
            return Result.Failure<UserEntity>(e.Message);
        }
    }
    
    
    public async Task<Result<UserEntity>> RegisterWithPasswordAsync(User user, string password, string roleStr, string? returnUrl = null)
    {
        var userExist = await _usersService.GetByEmailAsync<User>(user.Email);

        if (userExist.IsFailure)
        {
            return Result.Failure<UserEntity>(userExist.Error);
        }

        if (userExist.Value != null)
        {
            return Result.Failure<UserEntity>(AuthorizationServiceErrorMessages.EmailAlreadyInUseError.GetDescription());
        }

        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var userCreationResult = await _usersRepository.CreateWithPasswordAsync(user, password);

            if (userCreationResult.IsFailure)
            {
                throw new Exception(userCreationResult.Error);
            }

            var role = await _roleManager.FindByNameAsync(roleStr);
            if (role == null)
            {
                var roleResult = Role.Builder()
                    .SetName(roleStr)
                    .Build();

                if (roleResult.IsFailure)
                {
                    throw new Exception(roleResult.Error);
                }

                var roleCreationResult = await _rolesRepository.CreateAsync<string>(roleResult.Value);
                if (roleCreationResult.IsFailure)
                {
                    throw new Exception(roleCreationResult.Error);
                }
            }

            var addUserToRoleResult = await _userManager.AddToRoleAsync(userCreationResult.Value, roleStr);
            if (!addUserToRoleResult.Succeeded)
            {
                throw new Exception(addUserToRoleResult.Errors.ToString());
            }
            
            await _userManager.AddClaimAsync(userCreationResult.Value, new Claim("AuthorizationType", "Cookie"));

            //TODO
            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                var sendConfirmationTokenResult = await SendConfirmationTokenAsync(userCreationResult.Value, returnUrl);
                if (sendConfirmationTokenResult.IsFailure)
                {
                    throw new Exception(sendConfirmationTokenResult.Error);
                }
            }
            
            if (!_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                await SignInAsync(userCreationResult.Value);
            }

            await _transactionRepository.CommitTransactionAsync(transaction);
            
            
            return Result.Success(userCreationResult.Value);
        }
        catch (Exception e)
        {
            _authorizationLogger.LogError(e.Message);
            await _transactionRepository.RollbackTransactionAsync(transaction);
            return Result.Failure<UserEntity>(e.Message);
        }
    }

    #endregion


    #region SignIn

    private async Task<Result> SignInAsync(UserEntity userEntity, bool isPersistent = false)
    {
        try
        {
            await _signInManager.SignInAsync(userEntity, isPersistent: isPersistent);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }
    
    #endregion

    #region SignOut

    public async Task<Result> SignOut()
    {
        try
        {
            await _signInManager.SignOutAsync();
            foreach (var tokenType in Enum.GetValues(typeof(TokenTypes)).Cast<TokenTypes>())
            {
                var result = RemoveRefreshTokenToCookies(tokenType);
                if (result.IsFailure)
                {
                    throw new Exception(result.Error);
                }
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            _authorizationLogger.LogError(e.Message);
            return Result.Failure(e.Message);
        }
    }

    private Result RemoveRefreshTokenToCookies(TokenTypes tokenType)
    {
        try
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(tokenType.ToString());

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }

    #endregion

    private async Task<Result> SendConfirmationTokenAsync(UserEntity user, string? returnUrl = null)
    {
        try
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //Check id in UserEntity
            
            var callbackUrl = _navigationManager.GetUriWithQueryParameters(
                _navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = user.Id, ["code"] = code, ["returnUrl"] = returnUrl });


            var confirmationToken = ConfirmationToken.Builder()
                .SetToken(code)
                .SetUserId(user.Id)
                .SetConfirmationType(ConfirmationTypes.Email)
                .Build();

            if (confirmationToken.IsFailure)
            {
                throw new Exception(confirmationToken.Error);
            }
            
            var tokenSavingResult = await _confirmationTokensRepository.CreateAsync<ConfirmationTokenResponse>(confirmationToken.Value);
            if (tokenSavingResult.IsFailure)
            {
                throw new Exception(tokenSavingResult.Error);
            }
            
            await _emailSender.SendConfirmationLinkAsync(user, user.Email!, HtmlEncoder.Default.Encode(callbackUrl));
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result> VerifyEmailConfirmationTokenAsync(string userId, string token)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var userExist = await _userManager.FindByIdAsync(userId);
            
            if (userExist == null)
            {
                throw new Exception(AuthorizationServiceErrorMessages.UserNotFoundError.GetDescription());
            }

            var result = await CheckConfirmationToken(token, userId);

            if (result.IsFailure)
            {
                throw new Exception(result.Error);
            }
            
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var confirmationResult = await _userManager.ConfirmEmailAsync(userExist, code);
            if (!confirmationResult.Succeeded)
            {
                throw new Exception(AuthorizationServiceErrorMessages.EmailConfirmationFailedError.GetDescription());
            }

            var deletionResult = await _confirmationTokensRepository.DeleteByUserIdWithType(userExist.Id, ConfirmationTypes.Email);
            if (deletionResult.IsFailure)
            {
                throw new Exception(AuthorizationServiceErrorMessages.ConfirmationTokenDeletionFailedError.GetDescription());
            }

            await _transactionRepository.CommitTransactionAsync(transaction);
            return Result.Success();
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackTransactionAsync(transaction);
            return Result.Failure(e.Message);
        }
    }

    private async Task<Result> CheckConfirmationToken(string token, string userId)
    {
        var tokenExist = await _confirmationTokensRepository.GetByTokenAsync<ConfirmationTokenResponse>(token);
        if (tokenExist == null)
        {
            return Result.Failure(AuthorizationServiceErrorMessages.CodeNotFoundError.GetDescription());
        }

        if (tokenExist.UserId != userId)
        {
            return Result.Failure(AuthorizationServiceErrorMessages.CodeNotFoundError.GetDescription());
        }

        if (tokenExist.Expiration < DateTime.Now)
        {
            return Result.Failure(AuthorizationServiceErrorMessages.ConfirmationTokenExpiredError.GetDescription());
        }

        return Result.Success();
    }
}