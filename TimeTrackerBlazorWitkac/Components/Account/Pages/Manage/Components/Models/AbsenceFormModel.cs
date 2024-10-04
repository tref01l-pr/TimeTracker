using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;


namespace TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;

public class AbsenceFormParameters
{
    public UserResponse User { get; set; } = new UserResponse();
    public bool Disabled { get; set; }
    public bool IsOpen { get; set; }
    public DateTime StartTime { get; set; }
    public int? StartHour { get; set; }
    public int? StartMinute { get; set; }
    public DateTime? EndTime { get; set; }
    public int? EndHour { get; set; }
    public int? EndMinute { get; set; }
    public bool IsFullDate { get; set; } = true;
    public IList<AbsenceTypeResponse> types { get; set; } = new List<AbsenceTypeResponse>();
    public TimeSelectorModel<int> startHourSelector { get; set; }
    public TimeSelectorModel<int> endHourSelector { get; set; }
    public TimeSelectorModel<int> startMinuteSelector { get; set; }
    public TimeSelectorModel<int> endMinuteSelector { get; set; }
    public static readonly int[] HourValues = Enumerable.Range(0, 24).ToArray();
    public static readonly int[] MinValues = Enumerable.Range(0, 60).ToArray();
    public AbsenceFormParameters()
    {
        var timeSelectorService = new TimeSelectorService();
        startHourSelector = timeSelectorService.Create("Godzina rozpoczęcia", StartHour ?? 0, HourValues);
        endHourSelector = timeSelectorService.Create("Godzina zakończenia", EndHour ?? 0, HourValues);
        startMinuteSelector = timeSelectorService.Create("Minuta rozpoczęcia", StartMinute ?? 0, MinValues);
        endMinuteSelector = timeSelectorService.Create("Minuta zakończenia", EndMinute ?? 0, MinValues);
    }
}


