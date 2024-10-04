namespace TimeTrackerBlazorWitkac.Data.Entities.BaseEntities;

public abstract class DateTimeBaseEntity<T> : DateOnlyBaseEntity<T>
{
    public int StartHour { get; set; }
    public int StartMinute { get; set; }
    public int EndHour { get; set; }
    public int EndMinute { get; set; }
}
