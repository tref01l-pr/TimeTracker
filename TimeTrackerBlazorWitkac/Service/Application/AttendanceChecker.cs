using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Interfaces;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;

namespace TimeTrackerBlazorWitkac.Service.Application;

public class AttendanceChecker : IAttendanceChecker
{
    private readonly IAttendancesRepository _attendancesRepository;
    private readonly ILogger<AttendanceChecker> _logger;

    public AttendanceChecker(IAttendancesRepository attendancesRepository, ILogger<AttendanceChecker> logger)
    {
        _attendancesRepository = attendancesRepository;
        _logger = logger;
    }

    public async Task<Result> CheckStrangeActivity(int cardId, DateOnly startDate, DateOnly? endDate, TimeSpan startTime, TimeSpan endTime, int idToIgnore)
    {
        if (!endDate.HasValue)
        {
            return Result.Success();
        }

        DateTime startDateTime = CreateDateTime(startDate, startTime);
        DateTime endDateTime = CreateDateTime(endDate.Value, endTime);

        var lastAttendancesResult = await _attendancesRepository.GetFilteredPageAsync<Attendance>(
            predicate: a => a.UserCardId == cardId &&
                            a.StartDate >= endDate.Value.AddDays(-1) &&
                            a.EndDate <= endDate.Value &&
                            a.Id != idToIgnore);

        if (lastAttendancesResult.IsFailure)
        {
            return Result.Failure(lastAttendancesResult.Error);
        }

        IList<Attendance> lastAttendances = lastAttendancesResult.Value.Items;
        
        IList<Attendance> todayAttendances = FilterAttendancesByDay(lastAttendances, endDate.Value);
        IList<Attendance> yesterdayAttendances = FilterAttendancesByDay(lastAttendances, endDate.Value.AddDays(-1));

        int totalWorkedMinutesToday = SumWorkedMinutes(todayAttendances);
        int totalWorkedMinutesYesterday = SumWorkedMinutes(yesterdayAttendances);

        CalculateWorkedMinutes(startDate, endDate.Value, startDateTime, endDateTime, ref totalWorkedMinutesToday, ref totalWorkedMinutesYesterday);

        return ValidateResults(todayAttendances, yesterdayAttendances, totalWorkedMinutesToday, totalWorkedMinutesYesterday, startDate, endDate.Value);
    }

    public async Task<bool> CheckDateCollision(int userCardId, DateOnly startDate, int startHour, int startMinute,
        DateOnly? endDate, int endHour, int endMinute, int? idToIgnore = null) =>
        endDate.HasValue
            ? await _attendancesRepository.HasDateCollision(
                userCardId,
                startDate,
                startHour,
                startMinute,
                endDate.Value,
                endHour,
                endMinute,
                idToIgnore)
            : await _attendancesRepository.HasDateCollision(
                userCardId,
                startDate,
                startHour,
                startMinute,
                idToIgnore);

    private static DateTime CreateDateTime(DateOnly date, TimeSpan time) =>
        new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);

    private static IList<Attendance> FilterAttendancesByDay(IEnumerable<Attendance> attendances, DateOnly day) =>
        attendances.Where(a => a.StartDate.Day == day.Day).ToList();

    private void CalculateWorkedMinutes(DateOnly startDate, DateOnly endDate, DateTime startDateTime, DateTime endDateTime, ref int totalWorkedMinutesToday, ref int totalWorkedMinutesYesterday)
    {
        if (startDate.Day != endDate.Day)
        {
            totalWorkedMinutesYesterday += GetAttendanceDurationInMinutes(startDateTime, EndOfDay(startDateTime));
            totalWorkedMinutesToday += GetAttendanceDurationInMinutes(StartOfDay(endDateTime), endDateTime);
        }
        else
        {
            totalWorkedMinutesToday += GetAttendanceDurationInMinutes(startDateTime, endDateTime);
        }
    }

    private static Result ValidateResults(IList<Attendance> todayAttendances, IList<Attendance> yesterdayAttendances, int totalWorkedMinutesToday, int totalWorkedMinutesYesterday, DateOnly startDate, DateOnly endDate)
    {
        if (todayAttendances.Count >= Attendance.MaxDailyAttendanceLimit)
        {
            return Result.Failure($"{nameof(todayAttendances)} cannot exceed {Attendance.MaxDailyAttendanceLimit} entries");
        }

       

        if (totalWorkedMinutesToday > Attendance.MaxDailyWorkHours * 60)
        {
            return Result.Failure($"{nameof(totalWorkedMinutesToday)} cannot exceed {Attendance.MaxDailyWorkHours} hours");
        }


        if (startDate.Day != endDate.Day)
        {
            if (yesterdayAttendances.Count >= Attendance.MaxDailyAttendanceLimit)
            {
                return Result.Failure($"{nameof(yesterdayAttendances)} cannot exceed {Attendance.MaxDailyAttendanceLimit} entries");
            }
        
            if (totalWorkedMinutesYesterday > Attendance.MaxDailyWorkHours * 60)
            {
                return Result.Failure($"{nameof(totalWorkedMinutesYesterday)} cannot exceed {Attendance.MaxDailyWorkHours} hours");
            }
        }

        return Result.Success();
    }
    
    private int SumWorkedMinutes(IEnumerable<Attendance> attendances) =>
        attendances.Sum(a => GetAttendanceDurationInMinutes(a.GetStartDateTime(), a.GetEndDateTime()));
    
    private static DateTime StartOfDay(DateTime date) =>
        new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

    private static DateTime EndOfDay(DateTime date) =>
        new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

    private int GetAttendanceDurationInMinutes(DateTime startTime, DateTime? endTime)
    {
        if (!endTime.HasValue)
        {
            _logger.LogWarning("Attendance duration calculation failed: endTime cannot be null.");
            return 0;
        }

        return (int)(endTime.Value - startTime).TotalMinutes;
    }
}