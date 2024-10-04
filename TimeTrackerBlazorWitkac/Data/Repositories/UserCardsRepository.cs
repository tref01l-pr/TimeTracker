using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Repositories;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Data.Repositories.BaseRepositories;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.QueryTags;

namespace TimeTrackerBlazorWitkac.Data.Repositories;

/// <summary>
/// Repository class for managing user cards, including CRUD operations.
/// </summary>
public class UserCardsRepository : BaseCrudRepository<TimeTrackerDbContext, UserCardEntity, UserCard, int>, IUserCardsRepository
{
    /// <summary>
    /// Initializes a new instance of the repository with the specified context and mapper.
    /// </summary>
    /// <param name="contextFactory"></param>
    /// <param name="mapper">The object mapper.</param>
    /// <param name="context"></param>
    public UserCardsRepository(TimeTrackerDbContext context, IDbContextFactory<TimeTrackerDbContext> contextFactory, IMapper mapper)
        : base(context, contextFactory, mapper) { }
    
    /// <summary>
    /// Retrieves all user cards from the database.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user cards should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all user cards projected to the specified type.</returns>
    public async Task<IList<TProjectTo>> GetAllAsync<TProjectTo>() =>
        await _transientContext.UserCards
            .TagWith(UserCardQueryTags.GetAllUserCards.GetDescription())
            .AsNoTracking()
            .ProjectTo<TProjectTo>(_mapperConfig)
            .ToListAsync();

    /// <summary>
    /// Retrieves all user cards for a specific user identified by their user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose cards are to be retrieved.</param>
    /// <typeparam name="TProjectTo">The type to which the user cards should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of user cards for the specified user, projected to the specified type.</returns>
    public async Task<IList<TProjectTo>> GetByUserAsync<TProjectTo>(string userId) =>
        await _transientContext.UserCards
            .TagWith(UserCardQueryTags.GetUserCardsByUserId.GetDescription())
            .AsNoTracking()
            .Where(uc => uc.UserId == userId)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .ToListAsync();

    /// <summary>
    /// Retrieves a user card based on its number.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user card should be projected.</typeparam>
    /// <param name="number">The unique number of the user card to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the user card projected to the specified type, or null if not found.</returns>
    public async Task<TProjectTo?> GetByNumberAsync<TProjectTo>(string number) =>
        await _transientContext.UserCards
            .TagWith(UserCardQueryTags.GetUserCardByNumber.GetDescription())
            .Where(uc => uc.Number == number)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .FirstOrDefaultAsync();

    public async Task<IList<TProjectTo>> GetByUserIdAsync<TProjectTo>(string userId) =>
        await _transientContext.UserCards
            .TagWith(UserCardQueryTags.GetUserCardsByUserId.GetDescription())
            .Where(uc => uc.UserId == userId && uc.UserDeletedId == null)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .ToListAsync();

    /// <summary>
    /// Checks if a user card with the specified number exists in the database.
    /// </summary>
    /// <param name="cardNumber">The number of the user card to check.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains true if the user card exists; otherwise, false.</returns>
    public async Task<bool> IsExistByNumberAsync(string cardNumber) =>
        await _transientContext.UserCards
            .TagWith(UserCardQueryTags.IsUserCardExistByNumber.GetDescription())
            .AnyAsync(uc => uc.Number == cardNumber);
    
    /// <summary>
    /// Updates the user card information in the database.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the updated user card should be projected.</typeparam>
    /// <param name="model">The user card model to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a result indicating whether the update was successful.</returns>
    public override async Task<Result<TProjectTo>> UpdateAsync<TProjectTo>(UserCard model)
    {
        try
        {
            var userCard = await _persistentContext.UserCards.FirstOrDefaultAsync(uc => uc.Id == model.Id);
            if (userCard == null)
            {
                return Result.Failure<TProjectTo>(UserCardsRepositoryErrorMessages.UserCardNotFound.GetDescription());
            }

            _persistentContext.Entry(userCard).CurrentValues.SetValues(model);

            var modifiedProperties = _persistentContext.Entry(userCard).Properties
                .Where(p => p.IsModified)
                .Select(p => p.Metadata.Name)
                .ToList();

            if (modifiedProperties.Count != 1 || !modifiedProperties.Contains(nameof(userCard.Name)))
            {
                return Result.Failure<TProjectTo>(UserCardsRepositoryErrorMessages.CannotChangeNumber.GetDescription());
            }

            return await base.UpdateAsync<TProjectTo>(model);
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
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a <see cref="Result"/> with the new active status of the user card, or an error message if the operation fails.</returns>
    public async Task<Result<bool>> ToggleIsActiveAsync(int id)
    {
        var card = await _persistentContext.UserCards.FirstOrDefaultAsync(uc => uc.Id == id);
        if (card == null)
        {
            return Result.Failure<bool>(UserCardsRepositoryErrorMessages.UserCardNotFound.GetDescription());
        }

        card.IsActive = !card.IsActive;

        var result = await SaveAsync(_persistentContext);
        return !result.Value
            ? Result.Failure<bool>(UserCardsRepositoryErrorMessages.SaveChangesFailed.GetDescription())
            : card.IsActive;
    }

    /// <summary>
    /// Deletes a user card by ID with a failure result as deletion is not allowed through this method.
    /// </summary>
    /// <param name="id">The ID of the user card to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a failure message indicating that the deletion is not permitted.</returns>
    public override Task<Result> DeleteByIdAsync(int id) =>
        Task.FromResult(Result.Failure(UserCardsRepositoryErrorMessages.UserCardDeletionFailed.GetDescription()));
    
    /// <summary>
    /// Soft deletes a user card by marking it as inactive and recording the deletion information.
    /// </summary>
    /// <param name="adminId">The ID of the administrator performing the deletion.</param>
    /// <param name="cardId">The ID of the user card to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a <see cref="Result"/> indicating whether the deletion operation was successful, or an error message if it failed.</returns>
    public async Task<Result> DeleteByIdAsync(string adminId, int cardId)
    {
        var card = await _persistentContext.UserCards
            .FirstOrDefaultAsync(uc => uc.Id == cardId);

        if (card == null)
        {
            return Result.Failure(UserCardsRepositoryErrorMessages.UserCardNotFound.GetDescription());
        }

        card.UserDeletedId = adminId;
        card.DeletedAt = DateOnly.FromDateTime(DateTime.Now);
        card.IsActive = false;

        var result = await SaveAsync(_persistentContext);
        return !result.Value
            ? Result.Failure(UserCardsRepositoryErrorMessages.SaveChangesFailed.GetDescription())
            : Result.Success();
    }
}