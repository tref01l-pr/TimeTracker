using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;

/// <summary>
/// Defines operations related to managing user cards, including CRUD operations
/// and specific actions like retrieving, toggling active status, and deleting user cards.
/// </summary>
public interface IUserCardsService : ICrudService<UserCardEntity, UserCard, int>
{
    /// <summary>
    /// Retrieves all user cards from the database.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user cards should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a <see cref="Result"/> with a list of all user cards projected to the specified type.</returns>
    Task<Result<IList<TProjectTo>>> GetAllAsync<TProjectTo>();

    /// <summary>
    /// Retrieves all user cards for a specific user identified by their user ID.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user cards should be projected.</typeparam>
    /// <param name="userId">The ID of the user whose cards are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a <see cref="Result"/> with a list of user cards for the specified user, projected to the specified type.</returns>
    Task<Result<IList<TProjectTo>>> GetByUserAsync<TProjectTo>(string userId);

    /// <summary>
    /// Retrieves a user card based on its number.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user card should be projected.</typeparam>
    /// <param name="number">The unique number of the user card to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a <see cref="Result"/> with the user card projected to the specified type, or null if not found.</returns>
    Task<Result<TProjectTo?>> GetByNumberAsync<TProjectTo>(string number);

    /// <summary>
    /// Toggles the active status of a user card.
    /// </summary>
    /// <param name="id">The ID of the user card whose active status is to be toggled.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a <see cref="Result"/> with the new active status of the user card, or an error message if the operation fails.</returns>
    Task<Result<bool>> ToggleIsActiveAsync(int id);

    /// <summary>
    /// Soft deletes a user card by marking it as inactive and recording the deletion information.
    /// </summary>
    /// <param name="adminId">The ID of the administrator performing the deletion.</param>
    /// <param name="cardId">The ID of the user card to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a <see cref="Result"/> indicating whether the deletion operation was successful, or an error message if it failed.</returns>
    Task<Result> DeleteByIdAsync(string adminId, int cardId);
}