//using TimeTrackerBlazorWitkac.Client;

using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Radzen;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Data.Repositories.BaseRepositories;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.QueryTags;
using TimeTrackerBlazorWitkac.Service;

namespace TimeTrackerBlazorWitkac.Data.Repositories;

/// <summary>
/// Provides user-related functionalities such as user sign-in, creation, and existence checks.
/// </summary>
public class UsersRepository : BaseCrudRepository<TimeTrackerDbContext, UserEntity, User, string>, IUsersRepository
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly RoleManager<RoleEntity> _roleManager;
    private readonly ILogger<UsersRepository> _logger;
    private readonly NavigationManager _navManager;
    private readonly IUserStore<UserEntity> _userStore;
    private readonly IOptions<IdentityOptions> _options;
    private readonly IHttpContextAccessor _contextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersRepository"/> class with the specified dependencies.
    /// </summary>
    /// <param name="userManager">The UserManager instance for managing users.</param>
    /// <param name="signInManager">The SignInManager instance for signing in users.</param>
    /// <param name="roleManager">The RoleManager instance for managing roles.</param>
    /// <param name="logger">The ILogger instance for logging operations.</param>
    /// <param name="navManager">The NavigationManager instance for navigation operations.</param>
    /// <param name="userStore">The IUserStore instance for user storage operations.</param>
    /// <param name="context"></param>
    /// <param name="contextFactory"></param>
    /// <param name="options"></param>
    /// <param name="contextAccessor"></param>
    /// <param name="mapper"></param>
    public UsersRepository(
        UserManager<UserEntity> userManager,
        SignInManager<UserEntity> signInManager,
        RoleManager<RoleEntity> roleManager,
        ILogger<UsersRepository> logger,
        NavigationManager navManager,
        IUserStore<UserEntity> userStore,
        TimeTrackerDbContext context, 
        IDbContextFactory<TimeTrackerDbContext> contextFactory,
        IOptions<IdentityOptions> options,
        IHttpContextAccessor contextAccessor,
        IMapper mapper) : base(context, contextFactory, mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
        _navManager = navManager;
        _userStore = userStore;
        _options = options;
        _contextAccessor = contextAccessor;
    }
    
    /// <summary>
    /// Creates a new user entity with the specified email address.
    /// </summary>
    /// <param name="email">The email address for the new user.</param>
    /// <returns>A new instance of the <see cref="UserEntity"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user entity cannot be created.</exception>
    public UserEntity CreateUser(string email)
    {
        try
        {
            return new UserEntity { UserName = email, Email = email };
        }
        catch
        {
            throw new InvalidOperationException(
                $"Can't create an instance of '{nameof(UserEntity)}'. Ensure that '{nameof(UserEntity)}' is not an abstract class and has a parameterless constructor.");
        }
    }

    /// <summary>
    /// Checks if a user with the specified email address exists in the system.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    public async Task<bool> IsUserExistsByEmail(string email)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        return existingUser != null;
    }

    public async Task<bool> IsUserExistsById(string id)
    {
        var existingUser = await _userManager.FindByIdAsync(id);
        return existingUser != null;
    }

    /// <summary>
    /// Redirects the user to the Microsoft login page.
    /// </summary>
    public void RoutingToMsLogin()
    {
        try
        {
            _navManager.NavigateTo("Account/LoginToken");
            Console.WriteLine("Routing to MS Login...");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Signs in the specified user.
    /// </summary>
    /// <param name="userEntity">The user entity to sign in.</param>
    public async Task SignInUser(UserEntity? userEntity)
    {
        if (userEntity == null)
        {
            _logger.LogError("SignInUser called with null user.");
            return;
        }

        await _signInManager.SignInAsync(userEntity, isPersistent: true);
    }

    /// <summary>
    /// Signs in the user if they exist in the system based on their email address.
    /// </summary>
    /// <param name="email">The email address of the user to sign in.</param>
    public async Task SignInIfExists(string email)
    {
        var userExist = await IsUserExistsByEmail(email);
        if (!userExist)
        {
            return;
        }

        var existingUser = await _userManager.FindByEmailAsync(email);
        await SignInUserClaims(existingUser);
    }

    public async Task SignInUserClaims(UserEntity user)
    {
        if (user != null)
        {
            var existingClaims = await _userManager.GetClaimsAsync(user);
            var tokenClaim = existingClaims.FirstOrDefault(c => c.Type == "Token");

            _contextAccessor.HttpContext.Response.Cookies.Append("Token", tokenClaim.Value, new CookieOptions
            {
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(2),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                HttpOnly = true
            });

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Token", tokenClaim.Value)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
        }

        await _signInManager.SignInAsync(user, isPersistent: true);

    }

    /// <summary>
    /// Generates and stores a token for a user identified by their email.
    /// </summary>
    /// <param name="token">The token to be generated and stored.</param>
    /// <param name="email">The email of the user for whom the token is to be generated.</param>
    /// <exception cref="Exception">Thrown when the user is not found or when saving changes to the database fails.</exception>
    public async Task GenerateToken(string token, string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            var userToken = new IdentityUserToken<string>
            {
                UserId = user.Id,
                LoginProvider = "Default",
                Name = "Token",
                Value = token
            };
            _transientContext.UserTokens.Add(userToken);
            await _transientContext.SaveChangesAsync();
            await _userManager.AddClaimAsync(user, new Claim("Token", token));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Adds a claim containing a token to a user identified by their email.
    /// </summary>
    /// <param name="email">The email of the user to whom the claim is to be added.</param>
    /// <param name="token">The token to be added as a claim.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddClaimToUser(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        var claimFactory = new ClaimsFactory(_userManager, _roleManager, _options);
        var claimgGetUser = await _userManager.GetClaimsAsync(user);
    }
    
    
    /// <summary>
    /// Checks if a user with the specified email address exists in the system.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    public async Task<bool> IsExistsByEmailAsync(string email) =>
        await _transientContext.Users
            .TagWith(UserQueryTags.IsExistsByEmail.GetDescription())
            .AsNoTracking()
            .AnyAsync(u => u.Email == email);
    
    public async Task<bool> IsExistsByIdAsync(string id) =>
        await _transientContext.Users
            .TagWith(UserQueryTags.IsExistsById.GetDescription())
            .AsNoTracking()
            .AnyAsync(u => u.Id == id);
    

    public async Task<TProjectTo?> GetByEmailAsync<TProjectTo>(string? email) =>
        await _transientContext.Users
            .TagWith(UserQueryTags.GetByEmail.GetDescription())
            .AsNoTracking()
            .Where(u => u.Email == email)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .FirstOrDefaultAsync();

    public async Task<Result<UserEntity>> CreateWithPasswordAsync(User model, string password)
    {
        try
        {
            var user = Activator.CreateInstance<UserEntity>();
            user.UserName = model.UserName;
            user.Email = model.Email;
            
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return Result.Failure<UserEntity>(result.Errors.ToString());
            }

            return user;
        }
        catch (Exception e)
        {
            return Result.Failure<UserEntity>(e.Message);
        }
    }
    
    public async Task<Result<UserEntity>> CreateWithMsTokenAsync(User model)
    {
        try
        {
            var userEntity = new UserEntity{ UserName = model.UserName, Email = model.Email };
            userEntity.EmailConfirmed = true;
            var entity = await _persistentContext.Set<UserEntity>().AddAsync(userEntity);
            var result = await SaveAsync(_persistentContext);

            if (!result.Value)
            {
                return Result.Failure<UserEntity>($"Something went wrong during create {typeof(UserEntity)}");
            }

            return entity.Entity;
        }
        catch (Exception e)
        {
            return Result.Failure<UserEntity>(e.Message);
        }
    }

    public override Task<Result<TProjectTo>> CreateAsync<TProjectTo>(User model)
    {
        return Task.FromResult(Result.Failure<TProjectTo>("Use other methods to create User!"));
    }
}