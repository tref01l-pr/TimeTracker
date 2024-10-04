using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Data.Repositories.BaseRepositories;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.QueryTags;

namespace TimeTrackerBlazorWitkac.Data.Repositories;

/// <summary>
/// Repository for handling attendance records.
/// </summary>
public class AttendancesRepository : BaseCrudRepository<TimeTrackerDbContext, AttendanceEntity, Attendance, int>, IAttendancesRepository
{
    public AttendancesRepository(TimeTrackerDbContext context, IDbContextFactory<TimeTrackerDbContext> contextFactory, IMapper mapper)
        : base(context, contextFactory, mapper) { }
    
    /// <summary>
    /// Retrieves the most recent attendance record for the given card ID.
    /// If there is no ongoing attendance, the most recent completed record is returned.
    /// </summary>
    /// <param name="cardId">The ID of the user's card.</param>
    /// <returns>The most recent attendance record or null if none exists.</returns>
    public async Task<TProjectTo?> GetLastByCardIdAsync<TProjectTo>(int cardId) where TProjectTo : class
    {
        IList<AttendanceEntity> attendances =
            await _transientContext.Attendances
                .TagWith(AttendanceQueryTags.GetLastByCardId.GetDescription())
                .Where(a => a.UserCardId == cardId)
                .OrderByDescending(a => a.StartDate)
                .ThenByDescending(a => a.StartHour)
                .ThenByDescending(a => a.StartMinute)
                .ToListAsync();

        if (!attendances.Any())
        {
            return null; 
        }

        var attendance = attendances.FirstOrDefault(a => a.EndDate == null) ?? attendances[0];

        return await _transientContext.Attendances
            .TagWith(AttendanceQueryTags.GetLastByCardId.GetDescription())
            .Where(a => a.Id == attendance.Id)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Checks if there is a date collision with any existing attendance records, optionally ignoring a specified record.
    /// </summary>
    /// <param name="userCardId">The ID of the user's card.</param>
    /// <param name="startDate">The start date to check for collisions.</param>
    /// <param name="startHour">The start hour to check for collisions.</param>
    /// <param name="startMinute">The start minute to check for collisions.</param>
    /// <param name="idToIgnore">Optional ID to ignore during collision check.</param>
    /// <returns>True if there is a collision, otherwise false.</returns>
    public async Task<bool> HasDateCollision(int userCardId, DateOnly startDate, int startHour, int startMinute,
        int? idToIgnore = null) =>
        await _transientContext.Attendances
            .TagWith(AttendanceQueryTags.CheckCollisionForStartDate.GetDescription())
            .Where(a => a.UserCardId == userCardId && (idToIgnore == null || a.Id != idToIgnore))
            .AnyAsync(a =>
                a.EndDate == null ||
                a.EndDate > startDate ||
                (a.EndDate == startDate &&
                 (a.EndHour > startHour || (a.EndHour == startHour && a.EndMinute > startMinute)))
            );

    /// <summary>
    /// Checks if there is a date collision with any existing attendance records within a given time range, optionally ignoring a specified record.
    /// </summary>
    /// <param name="userCardId">The ID of the user's card.</param>
    /// <param name="startDate">The start date of the range to check for collisions.</param>
    /// <param name="startHour">The start hour of the range to check for collisions.</param>
    /// <param name="startMinute">The start minute of the range to check for collisions.</param>
    /// <param name="endDate">The end date of the range to check for collisions.</param>
    /// <param name="endHour">The end hour of the range to check for collisions.</param>
    /// <param name="endMinute">The end minute of the range to check for collisions.</param>
    /// <param name="idToIgnore">Optional ID to ignore during collision check.</param>
    /// <returns>True if there is a collision, otherwise false.</returns>
    public async Task<bool> HasDateCollision(int userCardId, DateOnly startDate, int startHour, int startMinute,
        DateOnly endDate, int endHour, int endMinute, int? idToIgnore = null)
    {
        var query = await _transientContext.Attendances
            .TagWith(AttendanceQueryTags.CheckCollisionForStartAndEndDates.GetDescription())
            .Where(a => a.UserCardId == userCardId && (idToIgnore == null || a.Id != idToIgnore))
            .ToArrayAsync();

        return query
            .Any(attendance =>
                IsDateCollision(attendance, startDate, startHour, startMinute, endDate, endHour, endMinute));
    }

    /// <summary>
    /// Determines if there is a date collision between an existing attendance record and the given date range.
    /// </summary>
    /// <param name="attendance">The existing attendance record.</param>
    /// <param name="startDate">The start date of the range to check for collisions.</param>
    /// <param name="startHour">The start hour of the range to check for collisions.</param>
    /// <param name="startMinute">The start minute of the range to check for collisions.</param>
    /// <param name="endDate">The end date of the range to check for collisions.</param>
    /// <param name="endHour">The end hour of the range to check for collisions.</param>
    /// <param name="endMinute">The end minute of the range to check for collisions.</param>
    /// <returns>True if there is a collision, otherwise false.</returns>
    private bool IsDateCollision(AttendanceEntity attendance, DateOnly startDate, int startHour, int startMinute,
        DateOnly endDate, int endHour, int endMinute)
    {
        bool endsBeforeStart = attendance.EndDate != null &&
                               (attendance.EndDate < startDate ||
                                (attendance.EndDate == startDate && attendance.EndHour < startHour) ||
                                (attendance.EndDate == startDate && attendance.EndHour == startHour &&
                                 attendance.EndMinute < startMinute));

        bool startsAfterEnd = attendance.StartDate > endDate ||
                              (attendance.StartDate == endDate && attendance.StartHour > endHour) ||
                              (attendance.StartDate == endDate && attendance.StartHour == endHour &&
                               attendance.StartMinute > endMinute);

        return !(endsBeforeStart || startsAfterEnd);
    }
}