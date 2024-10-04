using TimeTrackerBlazorWitkac.Data.Models;
using BlazorBootstrap;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;

public class WorkDayResponse
{
    public int Id { get; set; }
    public int? UserCardId { get; set; }
    public string? UserId { get; set; }
    public int? CompanyId { get; set; }
    public int? AbsenceTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public AbsenceTypeResponse? AbsenceType { get; set; }
    public AttendanceResponse? AttendanceResponse { get; set; } = new AttendanceResponse();
    public bool? IsFullDate { get; set; }
    public string? Reason { get; set; }
    public string? Name { get; set; }
    public bool? IsStrangeActivity { get; set; }
    public WorkDayType WorkDayType { get; set; }
    public ConfirmationStatus StatusOfType { get; set; }
    public ConfirmationStatus StatusOfDates { get; set; }
    public ConfirmationStatus IsFullyConfirmed { get; set; }

    public IList<IconStatus> icons { get; set; } = new List<IconStatus>
    {
        new IconStatus { Text = "Oczekuje", ConfirmationStatus = ConfirmationStatus.Pending, StatusOfDates = ConfirmationStatus.Pending, StatusOfType = ConfirmationStatus.Pending, IconName = IconName.Question },
        new IconStatus { Text = "Zaakceptowano", ConfirmationStatus = ConfirmationStatus.Confirmed, StatusOfDates = ConfirmationStatus.Confirmed, StatusOfType = ConfirmationStatus.Confirmed, IconName = IconName.Check2All },
        new IconStatus { Text = "Zaakceptowano DW", ConfirmationStatus = ConfirmationStatus.Pending, StatusOfDates = ConfirmationStatus.Confirmed, StatusOfType = ConfirmationStatus.Pending,  IconName = IconName.Check2},
        new IconStatus { Text = "Zaakceptowano RW", ConfirmationStatus = ConfirmationStatus.Pending, StatusOfDates = ConfirmationStatus.Pending, StatusOfType = ConfirmationStatus.Confirmed, IconName = IconName.Check2},
    };

    public UserResponse User { get; set; } = new UserResponse();

    public string TextByCalendarUser
    {
        get
        {
            if (WorkDayType == WorkDayType.Attendance)
            {
                var startTime = $"{StartDate.Hour:D2}:{StartDate.Minute:D2}";
                var endTime = $"{EndDate.Value.Hour:D2}:{EndDate.Value.Minute:D2}";
                return $"{startTime} - {endTime}";
            }
            else if (WorkDayType == WorkDayType.Holiday)
            {
                return Name ?? string.Empty;
            }
            else if (WorkDayType == WorkDayType.Absence)
            {
                return $"{Reason ?? string.Empty}";
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public string TextByCalendar
    {
        get
        {
            if (WorkDayType == WorkDayType.Holiday)
            {
                return Name ?? string.Empty;
            }
            else
            {
                return $"{User?.Email ?? string.Empty}";
            }
        }
    }
}