using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services.Application;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Service.Application.BaseServices;

namespace TimeTrackerBlazorWitkac.Service.Application;

/// <summary>
/// Provides operations for managing user cards, implementing CRUD functionality 
/// and additional features such as retrieving cards by user ID, toggling active status, 
/// and handling deletions with transaction support.
/// </summary>
public class UserCardsService : CrudService<IUserCardsRepository, UserCardEntity, UserCard, int>, IUserCardsService
{
    private readonly IUsersRepository _usersRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserCardsService"/> class.
    /// </summary>
    /// <param name="repository">The user cards repository.</param>
    /// <param name="usersRepository">The users repository.</param>
    /// <param name="transactionRepository">The transaction repository for managing transactions.</param>
    public UserCardsService(
        IUserCardsRepository repository,
        IUsersRepository usersRepository,
        ITransactionRepository transactionRepository) : base(repository, transactionRepository)
    {
        _usersRepository = usersRepository;
    }

    /// <summary>
    /// Retrieves all user cards.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user cards should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation, containing the list of user cards projected to the specified type.</returns>
    public async Task<Result<IList<TProjectTo>>> GetAllAsync<TProjectTo>()
    {
        try
        {
            var result = await _repository.GetAllAsync<TProjectTo>();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<IList<TProjectTo>>(e.Message);
        }
    }

    /// <summary>
    /// Retrieves all user cards for a specific user identified by their user ID.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user cards should be projected.</typeparam>
    /// <param name="userId">The ID of the user whose cards are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of user cards for the specified user projected to the specified type.</returns>
    public async Task<Result<IList<TProjectTo>>> GetByUserAsync<TProjectTo>(string userId)
    {
        try
        {
            var result = await _repository.GetByUserIdAsync<TProjectTo>(userId);
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<IList<TProjectTo>>(e.Message);
        }
    }
    
    /// <summary>
    /// Retrieves a user card based on its unique number.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user card should be projected.</typeparam>
    /// <param name="number">The unique number of the user card to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation, containing the user card projected to the specified type or null if not found.</returns>
    public async Task<Result<TProjectTo?>> GetByNumberAsync<TProjectTo>(string number)
    {
        try
        {
            var result = await _repository.GetByNumberAsync<TProjectTo>(number);
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo?>(e.Message);
        }
    }

    /// <summary>
    /// Creates a new user card.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user card should be projected.</typeparam>
    /// <param name="model">The user card model to create.</param>
    /// <returns>A task that represents the asynchronous operation, containing the created user card projected to the specified type.</returns>
    public override async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(UserCard model)
    {
        try
        {
            var cardByNumber = await _repository.GetByNumberAsync<TProjectTo>(model.Number);

            if (cardByNumber != null)
            {
                throw new Exception(UserCardsServiceErrorMessages.UserCardNumberAlreadyExists.GetDescription());
            }

            return await base.CreateAsync<TProjectTo>(model);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    /// <summary>
    /// Toggles the active status of a user card.
    /// </summary>
    /// <param name="id">The ID of the user card whose active status is to be toggled.</param>
    /// <returns>A task that represents the asynchronous operation, containing the new active status of the user card or an error message if the operation fails.</returns>
    public async Task<Result<bool>> ToggleIsActiveAsync(int id)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var result = await _repository.ToggleIsActiveAsync(id);
            if (result.IsFailure)
            {
                throw new Exception(result.Error);
            }

            await _transactionRepository.CommitTransactionAsync(transaction);
            return result.Value;
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackTransactionAsync(transaction);
            return Result.Failure<bool>(e.Message);
        }
    }

    /// <summary>
    /// Soft deletes a user card by marking it as inactive and recording the deletion information.
    /// </summary>
    /// <param name="adminId">The ID of the administrator performing the deletion.</param>
    /// <param name="cardId">The ID of the user card to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Result indicating whether the deletion operation was successful, or an error message if it failed.</returns>
    public async Task<Result> DeleteByIdAsync(string adminId, int cardId)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var card = await _repository.GetByIdAsync<UserCard>(cardId);
            if (card == null)
            {
                throw new Exception(UserCardsServiceErrorMessages.UserCardNotFound.GetDescription());
            }

            var userAdmin = await _usersRepository.GetByIdAsync<UserResponse>(adminId);
            if (userAdmin == null)
            {
                throw new Exception(UserCardsServiceErrorMessages.UserNotFound.GetDescription());
            }

            if (!userAdmin.Roles.Contains(nameof(Roles.Admin)))
            {
                throw new Exception(UserCardsServiceErrorMessages.UserNotAdmin.GetDescription());
            }

            var result = await _repository.DeleteByIdAsync(adminId, cardId);
            if (result.IsFailure)
            {
                throw new Exception(result.Error);
            }

            await _transactionRepository.CommitTransactionAsync(transaction);
            return result;
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackTransactionAsync(transaction);
            return Result.Failure(e.Message);
        }
    }

    /// <summary>
    /// Prevents the direct deletion of a user card by ID.
    /// </summary>
    /// <param name="id">The ID of the user card to delete.</param>
    /// <returns>A Result indicating that this method cannot be used.</returns>
    public override Task<Result> DeleteByIdAsync(int id)
    {
        return Task.FromResult(Result.Failure("You can't use this method. User another one!"));
    }
}