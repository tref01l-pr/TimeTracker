using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Options;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;

/// <summary>
/// Defines operations related to managing absence records, including CRUD operations.
/// </summary>
public interface IAbsencesService : ICrudService<AbsenceEntity, Absence, int>
{
    /// <summary>
    /// Retrieves all absence records from the database.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the absence records should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result with a list of all absence records projected to the specified type.</returns>
    Task<Result<IList<TProjectTo>>> GetAllAsync<TProjectTo>();
    
    /// <summary>
    /// Retrieves all absence records for a specific user identified by their ID.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the absence records should be projected.</typeparam>
    /// <param name="userId">The ID of the user whose absences are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result with a list of absence records for the specified user, projected to the specified type.</returns>
    Task<Result<IList<TProjectTo>>> GetByUserIdAsync<TProjectTo>(string userId);

    /// <summary>
    /// Retrieves a filtered list of absence records based on specified criteria.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the absence records should be projected.</typeparam>
    /// <param name="options">Optional; pagination options for the result set.</param>
    /// <param name="id">Optional; the ID of the absence record to filter by.</param>
    /// <param name="userId">Optional; the ID of the user to filter absences by.</param>
    /// <param name="startDate">Optional; the start date for filtering absences.</param>
    /// <param name="endDate">Optional; the end date for filtering absences.</param>
    /// <param name="cancellationToken">Optional; a cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result with a paginated response of absence records filtered by the specified criteria, projected to the specified type.</returns>
    Task<Result<PageResponse<TProjectTo>>> GetFilteredAsync<TProjectTo>(
        PaginationOptions? options = null,
        int? id = null,
        string? userId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default);

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
