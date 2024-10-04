using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Interfaces;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;
using TimeTrackerBlazorWitkac.Helpers;

namespace TimeTrackerBlazorWitkac.Data.Models;

// The Absence record class represents an absence.
// It provides a structured way to work with absence data in the application.
public record Absence : IModelKey<int>
{
    public const int MaxReasonLength = 360;

    // Private constructor to enforce the use of the Create method for object creation.
    private Absence(
        int id,
        string userId,
        int absenceTypeId,
        ConfirmationStatus statusOfType,
        ConfirmationStatus statusOfDates,
        ConfirmationStatus isFullyConfirmed,
        DateOnly startDate,
        int startHour,
        int startMinute,
        DateOnly endDate,
        int endHour,
        int endMinute,
        bool isFullDate,
        string? reason)
    {
        Id = id;
        UserId = userId;
        AbsenceTypeId = absenceTypeId;
        StatusOfType = statusOfType;
        StatusOfDates = statusOfDates;
        IsFullyConfirmed = isFullyConfirmed;
        StartDate = startDate;
        StartHour = startHour;
        StartMinute = startMinute;
        EndDate = endDate;
        EndHour = endHour;
        EndMinute = endMinute;
        IsFullDate = isFullDate;
        Reason = reason;
    }

    public int Id { get; init; }
    public string UserId { get; }
    public int AbsenceTypeId { get; }
    public ConfirmationStatus StatusOfType { get; }
    public ConfirmationStatus StatusOfDates { get; }
    public ConfirmationStatus IsFullyConfirmed { get; }
    public DateOnly StartDate { get; }
    public int StartHour { get; }
    public int StartMinute { get; }
    public DateOnly EndDate { get; }
    public int EndHour { get; }
    public int EndMinute { get; }
    public bool IsFullDate { get; }
    public string? Reason { get; }
    public static AbsenceBuilder Builder() => new AbsenceBuilder();

    public class AbsenceBuilder
    {
        private int _id;
        private string _userId;
        private int _absenceTypeId;
        private DateOnly _startDate;
        private ConfirmationStatus _statusOfType;
        private ConfirmationStatus _statusOfDates;
        private int _startHour;
        private int _startMinute;
        private DateOnly _endDate;
        private int _endHour;
        private int _endMinute;
        private bool _isFullDate;
        private string? _reason;

        public AbsenceBuilder SetId(int id)
        {
            _id = id;
            return this;
        }

        public AbsenceBuilder SetUserId(string userId)
        {
            _userId = userId;
            return this;
        }

        public AbsenceBuilder SetAbsenceTypeId(int absenceTypeId)
        {
            _absenceTypeId = absenceTypeId;
            return this;
        }

        public AbsenceBuilder SetStartDate(DateOnly startDate)
        {
            _startDate = startDate;
            return this;
        }

        public AbsenceBuilder SetStartHour(int startHour)
        {
            _startHour = startHour;
            return this;
        }

        public AbsenceBuilder SetStartMinute(int startMinute)
        {
            _startMinute = startMinute;
            return this;
        }
        public AbsenceBuilder SetStatusOfType(ConfirmationStatus status) {

            _statusOfType = status;
            return this;
        }
        public AbsenceBuilder SetStatusOfDate(ConfirmationStatus status)
        {
            _statusOfDates = status;
            return this;
        }
        public AbsenceBuilder SetEndDate(DateOnly endDate)
        {
            _endDate = endDate;
            return this;
        }

        public AbsenceBuilder SetEndHour(int endHour)
        {
            _endHour = endHour;
            return this;
        }

        public AbsenceBuilder SetEndMinute(int endMinute)
        {
            _endMinute = endMinute;
            return this;
        }

        public AbsenceBuilder SetIsFullDate(bool isFullDate)
        {
            _isFullDate = isFullDate;
            return this;
        }

        public AbsenceBuilder SetReason(string? reason)
        {
            _reason = reason;
            return this;
        }

        public Result<Absence> Build()
        {
            var validationResult = ValidateAbsenceData(_userId, _absenceTypeId, _startDate,  _endDate,
                _isFullDate, _startHour, _startMinute, _endHour, _endMinute, _reason);

            if (validationResult.IsFailure)
            {
                return Result.Failure<Absence>(validationResult.Error);
            }

            if (_isFullDate)
            {
                ResetTimeFields(ref _startHour, ref _startMinute, ref _endHour, ref _endMinute);
            }

            _reason = NormalizeReason(_reason);

            return new Absence(_id, _userId, _absenceTypeId, ConfirmationStatus.Pending, 
                ConfirmationStatus.Pending, ConfirmationStatus.Pending, _startDate, 
                _startHour, _startMinute, _endDate, _endHour, _endMinute, _isFullDate, _reason);
        }
    }

    private static Result ValidateAbsenceData(
        string userId,
        int absenceTypeId,
        DateOnly startDate,
        DateOnly endDate,
        bool isFullDate,
        int startHour,
        int startMinute,
        int endHour,
        int endMinute,
        string? reason)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure(AbsenceErrorMessages.InvalidUserId.GetDescription());
        }

        if (absenceTypeId <= 0)
        {
            return Result.Failure(AbsenceErrorMessages.InvalidAbsenceTypeId.GetDescription());
        }

        if (isFullDate)
        {
            if (startDate > endDate)
            {
                return Result.Failure(AbsenceErrorMessages.InvalidDateRangeForFullDayAbsence.GetDescription());
            }
        }
        else
        {
            var timeValidationResult =
                ValidateTimeFields(startDate, endDate, startHour, startMinute, endHour, endMinute);
            if (timeValidationResult.IsFailure)
            {
                return timeValidationResult;
            }
        }

        if (reason != null && reason.Length >= MaxReasonLength)
        {
            return Result.Failure<Absence>(AbsenceErrorMessages.ReasonTooLong.GetDescription());
        }

        return Result.Success();
    }

    private static Result ValidateTimeFields(
        DateOnly startDate,
        DateOnly endDate,
        int startHour,
        int startMinute,
        int endHour,
        int endMinute)
    {
        if (startDate != endDate)
        {
            return Result.Failure(AbsenceErrorMessages.StartDateMustEqualEndDateForNonFullDayAbsence
                .GetDescription());
        }
        
        if (!TimeValidator.ValidateTime(startHour, startMinute, out TimeSpan startTime) ||
            !TimeValidator.ValidateTime(endHour, endMinute, out TimeSpan endTime))
        {
            return Result.Failure(AbsenceErrorMessages.InvalidTimeRange.GetDescription());
        }

        if (startTime >= endTime)
        {
            return Result.Failure(AbsenceErrorMessages.StartTimeMustBeLessThanEndTime.GetDescription());
        }

        return Result.Success();
    }

    private static void ResetTimeFields(ref int startHour, ref int startMinute, ref int endHour, ref int endMinute)
    {
        startHour = startMinute = 0;
        endHour = 23;
        endMinute = 59;
    }

    private static string? NormalizeReason(string? reason)
    {
        return string.IsNullOrWhiteSpace(reason) ? null : reason;
    }
}