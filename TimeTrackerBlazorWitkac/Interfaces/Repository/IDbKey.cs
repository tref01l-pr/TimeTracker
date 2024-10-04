namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

public interface IDbKey<TKey>
{
    TKey Id { get; set; }
}