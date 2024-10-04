using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

/// <summary>
/// Defines operations related to managing user cards, including CRUD operations.
/// </summary>
public interface IUserCardsRepository : ICrudRepository<UserCardEntity, UserCard, int>
{
    /// <summary>
    /// Retrieves all user cards from the database.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user cards should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a list of all user cards projected to the specified type.</returns>
    Task<IList<TProjectTo>> GetAllAsync<TProjectTo>();

    /// <summary>
    /// Retrieves a user card based on its number.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the user card should be projected.</typeparam>
    /// <param name="number">The unique number of the user card to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the user card projected to the specified type, or null if not found.</returns>
    Task<TProjectTo?> GetByNumberAsync<TProjectTo>(string number);

    /// <summary>
    /// Retrieves all user cards for a specific user identified by their user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose cards are to be retrieved.</param>
    /// <typeparam name="TProjectTo">The type to which the user cards should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a list of user cards for the specified user, projected to the specified type.</returns>
    Task<IList<TProjectTo>> GetByUserIdAsync<TProjectTo>(string userId);

    /// <summary>
    /// Checks if a user card with the specified number exists in the database.
    /// </summary>
    /// <param name="number">The number of the user card to check for existence.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains true if the user card exists; otherwise, false.</returns>
    Task<bool> IsExistByNumberAsync(string number);

    /// <summary>
    /// Toggles the active status of a user card by its ID.
    /// </summary>
    /// <param name="id">The ID of the user card whose active status is to be toggled.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a <see cref="Result"/> indicating whether the operation was successful, 
    /// along with the new active status of the user card.</returns>
    Task<Result<bool>> ToggleIsActiveAsync(int id);

    /// <summary>
    /// Soft deletes a user card by marking it as inactive and recording the deletion information.
    /// </summary>
    /// <param name="adminId">The ID of the administrator performing the deletion.</param>
    /// <param name="cardId">The ID of the user card to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains a <see cref="Result"/> indicating whether the deletion operation was successful.</returns>
    Task<Result> DeleteByIdAsync(string adminId, int cardId);
}