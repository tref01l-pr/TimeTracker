using TimeTrackerBlazorWitkac.Interfaces;
using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;
using TimeTrackerBlazorWitkac.Helpers;

namespace TimeTrackerBlazorWitkac.Data.Models;

// The Attendance record class represents an absence.
// It provides a structured way to work with attendance data in the application.
public record Attendance : IModelKey<int>
{
    public static int MaxStrangeActivityReasonLength = 360;
    public static int MaxDailyAttendanceLimit = 10;
    public static int MaxDailyWorkHours = 10;

    private Attendance(
        int id,
        int userCardId,
        string userId,
        int companyId,
        DateOnly startDate,
        int startHour,
        int startMinute,
        DateOnly? endDate,
        int endHour,
        int endMinute,
        bool isStrangeActivity,
        string? strangeActivityReason,
        DateTime? resolvedAt,
        string? resolvedById)
    {
        Id = id;
        UserCardId = userCardId;
        UserId = userId;
        CompanyId = companyId;
        StartDate = startDate;
        StartHour = startHour;
        StartMinute = startMinute;
        EndDate = endDate;
        EndHour = endHour;
        EndMinute = endMinute;
        IsStrangeActivity = isStrangeActivity;
        StrangeActivityReason = strangeActivityReason;
        ResolvedAt = resolvedAt;
        ResolvedById = resolvedById;
    }

    public int Id { get; init; }
    public int UserCardId { get; private set; }
    public string UserId { get; private set; }
    public int CompanyId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public int StartHour { get; private set; }
    public int StartMinute { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public int EndHour { get; private set; }
    public int EndMinute { get; private set; }
    public bool IsStrangeActivity { get; private set; }
    public string? StrangeActivityReason { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolvedById { get; private set; }

    public DateTime GetStartDateTime() =>
        new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartHour, StartMinute, 0);

    public DateTime? GetEndDateTime() => EndDate.HasValue
        ? new DateTime(EndDate.Value.Year, EndDate.Value.Month, EndDate.Value.Day, EndHour, EndMinute, 0)
        : null;


    public static AttendanceBuilder Builder() => new AttendanceBuilder();

    public class AttendanceBuilder
    {
        private int _id;
        private int _userCardId;
        private string _userId = string.Empty;
        private int _companyId;
        private DateOnly _startDate;
        private int _startHour;
        private int _startMinute;
        private DateOnly? _endDate;
        private int _endHour;
        private int _endMinute;
        private bool _isStrangeActivity;
        private string? _strangeActivityReason;
        private DateTime? _resolvedAt;
        private string? _resolvedById;

        public AttendanceBuilder SetId(int id)
        {
            _id = id;
            return this;
        }

        public AttendanceBuilder SetUserCardId(int userCardId)
        {
            _userCardId = userCardId;
            return this;
        }

        public AttendanceBuilder SetUserId(string userId)
        {
            _userId = userId;
            return this;
        }

        public AttendanceBuilder SetCompanyId(int companyId)
        {
            _companyId = companyId;
            return this;
        }

        public AttendanceBuilder SetStartDate(DateOnly startDate)
        {
            _startDate = startDate;
            return this;
        }

        public AttendanceBuilder SetStartHour(int startHour)
        {
            _startHour = startHour;
            return this;
        }

        public AttendanceBuilder SetStartMinute(int startMinute)
        {
            _startMinute = startMinute;
            return this;
        }

        public AttendanceBuilder SetEndDate(DateOnly? endDate)
        {
            _endDate = endDate;
            return this;
        }

        public AttendanceBuilder SetEndHour(int endHour)
        {
            _endHour = endHour;
            return this;
        }

        public AttendanceBuilder SetEndMinute(int endMinute)
        {
            _endMinute = endMinute;
            return this;
        }

        public AttendanceBuilder SetIsStrangeActivity(bool isStrangeActivity)
        {
            _isStrangeActivity = isStrangeActivity;
            return this;
        }

        public AttendanceBuilder SetStrangeActivityReason(string? strangeActivityReason)
        {
            _strangeActivityReason = strangeActivityReason;
            return this;
        }

        public AttendanceBuilder SetResolvedAt(DateTime? resolvedAt)
        {
            _resolvedAt = resolvedAt;
            return this;
        }

        public AttendanceBuilder SetResolvedById(string? resolvedById)
        {
            _resolvedById = resolvedById;
            return this;
        }

        public Result<Attendance> Build()
        {
            var validationResult = ValidateAttendanceData(
                _userCardId, _userId, _companyId, _startDate, _startHour, _startMinute,
                _endDate, _endHour, _endMinute, _isStrangeActivity, _strangeActivityReason,
                _resolvedAt, _resolvedById);

            if (validationResult.IsFailure)
            {
                return Result.Failure<Attendance>(validationResult.Error);
            }

            if (!_isStrangeActivity)
            {
                ResetStrangeActivity(ref _strangeActivityReason);
            }

            _strangeActivityReason = NormalizeValue(_strangeActivityReason);
            _resolvedById = NormalizeValue(_resolvedById);

            return Result.Success(new Attendance(
                _id, _userCardId, _userId, _companyId, _startDate, _startHour, _startMinute,
                _endDate, _endHour, _endMinute, _isStrangeActivity, _strangeActivityReason,
                _resolvedAt, _resolvedById));
        }
    }

    private static Result ValidateAttendanceData(
        int userCardId,
        string userId,
        int companyId,
        DateOnly startDate,
        int startHour,
        int startMinute,
        DateOnly? endDate,
        int endHour,
        int endMinute,
        bool isStrangeActivity,
        string? strangeActivityReason,
        DateTime? resolvedAt,
        string? resolvedById)
    {
        if (userCardId <= 0)
            return Result.Failure<Attendance>(AttendanceErrorMessages.InvalidUserCardId.GetDescription());

        if (string.IsNullOrWhiteSpace(userId))
            return Result.Failure<Attendance>(AttendanceErrorMessages.InvalidUserId.GetDescription());

        if (companyId <= 0)
            return Result.Failure<Attendance>(AttendanceErrorMessages.InvalidCompanyId.GetDescription());

        var timeValidationResult = ValidateTimeFields(startDate, endDate, startHour, startMinute, endHour, endMinute);

        if (timeValidationResult.IsFailure)
            return timeValidationResult;

        var strangeActivityValidationResult =
            ValidateStrangeActivity(isStrangeActivity, strangeActivityReason, resolvedAt, resolvedById);

        if (strangeActivityValidationResult.IsFailure)
        {
            return strangeActivityValidationResult;
        }

        var resolveValidationResult = ValidateResolve(resolvedAt, resolvedById);

        if (resolveValidationResult.IsFailure)
        {
            return resolveValidationResult;
        }

        return strangeActivityValidationResult.IsFailure
            ? strangeActivityValidationResult
            : Result.Success();
    }

    private static Result ValidateResolve(DateTime? resolvedAt, string? resolvedById)
    {
        if (!resolvedAt.HasValue != string.IsNullOrWhiteSpace(resolvedById))
            return Result.Failure<Attendance>(AttendanceErrorMessages.InvalidResolvedAtAndBy.GetDescription());

        return Result.Success();
    }

    private static Result ValidateTimeFields(
        DateOnly startDate,
        DateOnly? endDate,
        int startHour,
        int startMinute,
        int endHour,
        int endMinute)
    {
        if (!TimeValidator.ValidateTime(startHour, startMinute, out TimeSpan startTime))
            return Result.Failure(AttendanceErrorMessages.InvalidStartTime.GetDescription());
        
        if (startDate.ToDateTime(new TimeOnly(startHour, startMinute, 0)) > DateTime.Now)
        {
            return Result.Failure(AttendanceErrorMessages.StartTimeMustBeLessThanRealTime.GetDescription());
        }
        
        if (endDate.HasValue)
        {
            if (startDate > endDate || !TimeValidator.ValidateTime(endHour, endMinute, out TimeSpan endTime))
            {
                return Result.Failure<Attendance>(AttendanceErrorMessages.InvalidEndDate.GetDescription());
            }

            if (startDate == endDate && startTime > endTime)
            {
                return Result.Failure<Attendance>(AttendanceErrorMessages.StartTimeMustBeLessThanEndTime.GetDescription());
            }
            
            if (startDate.ToDateTime(new TimeOnly(endHour, endMinute, 0)) > DateTime.Now)
            {
                return Result.Failure(AttendanceErrorMessages.EndTimeMustBeLessThanRealTime.GetDescription());
            }
        }

        return Result.Success();
    }

    private static bool IsValidTime(int hour, int minute) =>
        hour is >= 0 and < 24 && minute is >= 0 and < 60;

    private static Result ValidateStrangeActivity(bool isStrangeActivity, string? strangeActivityReason,
        DateTime? resolvedAt, string? resolvedById)
    {
        if (!isStrangeActivity) return Result.Success();

        if (strangeActivityReason?.Length > MaxStrangeActivityReasonLength)
            return Result.Failure<Attendance>(AttendanceErrorMessages.StrangeActivityReasonTooLong.ToString());

        return Result.Success();
    }

    private static void ResetStrangeActivity(ref string? strangeActivityReason)
    {
        strangeActivityReason = null;
    }

    private static string? NormalizeValue(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}