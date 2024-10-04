using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Contracts.Requests;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Options;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;
/// <summary>
/// Interface for managing attendance-related operations in the system.
/// </summary>
public interface IAttendancesService : ICrudService<AttendanceEntity, Attendance, int>
{
    /// <summary>
    /// Asynchronously retrieves a filtered list of attendance records with support for result projection.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the result to.</typeparam>
    /// <param name="options">Pagination options (optional).</param>
    /// <param name="userCardId">User card ID for filtering (optional).</param>
    /// <param name="userId">User ID for filtering (optional).</param>
    /// <param name="startDate">Start date for filtering (optional).</param>
    /// <param name="endDate">End date for filtering (optional).</param>
    /// <param name="idToIgnore">An ID to exclude from the results (optional).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request (optional).</param>
    /// <returns>A task that returns the result of the operation with the filtered and projected data.</returns>
    public Task<Result<PageResponse<TProjectTo>>> GetFilteredAsync<TProjectTo>(
        PaginationOptions? options = null,
        int? userCardId = null,
        string? userId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        int? idToIgnore = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the last attendance record by the specified user card ID.
    /// </summary>
    /// <param name="userCardId">The ID of the user card.</param>
    /// <returns>A task that returns the last attendance record for the given user card.</returns>
    Task<Result<TProjectTo?>> GetLastByCardIdAsync<TProjectTo>(int userCardId) where TProjectTo : class;

    /// <summary>
    /// Processes a card punch operation using the specified card number.
    /// </summary>
    /// <param name="number">The card number used for the punch operation.</param>
    /// <param name="actionTime"></param>
    /// <typeparam name="TProjectTo">The type of the result to return.</typeparam>
    /// <returns>A task that returns the result of the operation, including success or failure status and the processed data.</returns>
    Task<Result<TProjectTo>> CardPunchAsync<TProjectTo>(string number, DateTime actionTime);

    /// <summary>
    /// Creates a new attendance record with admin privileges based on the provided request.
    /// </summary>
    /// <param name="attendanceCreateRequest">The request containing the details for creating the attendance record.</param>
    /// <typeparam name="TProjectTo">The type of the result to return.</typeparam>
    /// <returns>A task that returns the result of the operation, including success or failure status and the processed data.</returns>
    Task<Result<TProjectTo>> CreateAsync<TProjectTo>(AdminAttendanceCreateRequest attendanceCreateRequest);

    /// <summary>
    /// Updates or resolves an attendance record with admin privileges based on the provided request.
    /// </summary>
    /// <param name="attendanceUpdateRequest">The request containing the details for updating the attendance record.</param>
    /// <param name="forceResolve">A flag indicating whether to forcefully resolve conflicts.</param>
    /// <typeparam name="TProjectTo">The type of the result to return.</typeparam>
    /// <returns>A task that returns the result of the operation, including success or failure status and the processed data.</returns>
    public Task<Result<TProjectTo>> UpdateOrResolveAsync<TProjectTo>(
        AdminAttendanceUpdateRequest attendanceUpdateRequest, bool forceResolve = false);
}
