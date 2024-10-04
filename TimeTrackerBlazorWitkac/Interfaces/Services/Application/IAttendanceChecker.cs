using CSharpFunctionalExtensions;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;

public interface IAttendanceChecker
{
    Task<Result> CheckStrangeActivity(int cardId, DateOnly startDate, DateOnly? endDate, TimeSpan startTime, 
        TimeSpan endTime, int idToIgnore);

    Task<bool> CheckDateCollision(int userCardId, DateOnly startDate, int startHour, int startMinute,
        DateOnly? endDate, int endHour, int endMinute, int? idToIgnore = null);
}