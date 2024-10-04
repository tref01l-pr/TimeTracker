using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

/// <summary>
/// Interface for managing attendance records in the repository.
/// Provides methods to retrieve, create, update, and delete attendance data,
/// as well as check for date collisions and fetch the most recent attendance by card ID.
/// </summary>
public interface IAttendancesRepository : ICrudRepository<AttendanceEntity, Attendance, int>
{
    /// <summary>
    /// Retrieves the most recent attendance record associated with the specified card ID.
    /// </summary>
    /// <param name="cardId">The ID of the card associated with the attendance record.</param>
    /// <returns>The most recent attendance record for the specified card ID, or null if no record is found.</returns>
    Task<TProjectTo?> GetLastByCardIdAsync<TProjectTo>(int cardId) where TProjectTo : class;

    /// <summary>
    /// Checks for a date collision with existing attendance records based on the specified start time.
    /// </summary>
    /// <param name="userCardId">The ID of the user's card to check for collisions.</param>
    /// <param name="startDate">The start date to check for collisions.</param>
    /// <param name="startHour">The start hour to check for collisions.</param>
    /// <param name="startMinute">The start minute to check for collisions.</param>
    /// <param name="idToIgnore">
    /// Optional ID of the attendance record to ignore during collision checks. 
    /// This is useful when updating an existing record to avoid false positives.
    /// </param>
    /// <returns>True if a collision is detected, otherwise false.</returns>
    Task<bool> HasDateCollision(int userCardId, DateOnly startDate, int startHour, int startMinute, int? idToIgnore = null);

    /// <summary>
    /// Checks for a date collision with existing attendance records based on the specified time range.
    /// </summary>
    /// <param name="userCardId">The ID of the user's card to check for collisions.</param>
    /// <param name="startDate">The start date of the time range to check for collisions.</param>
    /// <param name="startHour">The start hour of the time range to check for collisions.</param>
    /// <param name="startMinute">The start minute of the time range to check for collisions.</param>
    /// <param name="endDate">The end date of the time range to check for collisions.</param>
    /// <param name="endHour">The end hour of the time range to check for collisions.</param>
    /// <param name="endMinute">The end minute of the time range to check for collisions.</param>
    /// <param name="idToIgnore">
    /// Optional ID of the attendance record to ignore during collision checks.
    /// This can be used when updating an existing record to prevent it from colliding with itself.
    /// </param>
    /// <returns>True if a collision is detected, otherwise false.</returns>
    Task<bool> HasDateCollision(int userCardId,
        DateOnly startDate, int startHour, int startMinute,
        DateOnly endDate, int endHour, int endMinute,
        int? idToIgnore = null);
}
