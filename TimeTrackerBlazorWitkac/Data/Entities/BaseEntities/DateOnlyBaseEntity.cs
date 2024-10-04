namespace TimeTrackerBlazorWitkac.Data.Entities.BaseEntities;

public abstract class DateOnlyBaseEntity<T> : BaseEntity<T>
{
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

}


