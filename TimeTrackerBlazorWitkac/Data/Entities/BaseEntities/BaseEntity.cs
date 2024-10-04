using TimeTrackerBlazorWitkac.Interfaces.Repository;

namespace TimeTrackerBlazorWitkac.Data.Entities.BaseEntities;

public abstract class BaseEntity<T> : IDbKey<T>
{
    public T Id { get; set; }
}