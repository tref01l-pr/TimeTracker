using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;


namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

/// <summary>
/// Defines operations related to managing absence records, including CRUD operations.
/// </summary>
public interface IAbsencesRepository : ICrudRepository<AbsenceEntity, Absence, int>
{
    /// <summary>
    /// Retrieves all absence records from the database.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the absence records should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all absence records projected to the specified type.</returns>
    Task<IList<TProjectTo>> GetAllAsync<TProjectTo>();

    /// <summary>
    /// Retrieves all absence records for a specific user identified by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose absences are to be retrieved.</param>
    /// <typeparam name="TProjectTo">The type to which the absence records should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of absence records for the specified user, projected to the specified type.</returns>
    Task<IList<TProjectTo>> GetByUserIdAsync<TProjectTo>(string userId);
    
    /// <summary>
    /// Toggles the status of an absence record by its ID.
    /// </summary>
    /// <param name="id">The ID of the absence record whose status is to be toggled.</param>
    /// <param name="statusOfType">Optional; the status of type to be set for the absence record.</param>
    /// <param name="statusOfDates">Optional; the status of dates to be set for the absence record.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result indicating whether the operation was successful.</returns>
    Task<Result<TProjectTo>> ToggleStatusById<TProjectTo>(int id, ConfirmationStatus? statusOfType = null, 
        ConfirmationStatus? statusOfDates = null);
}

