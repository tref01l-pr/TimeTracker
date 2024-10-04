namespace TimeTrackerBlazorWitkac.Interfaces;

public interface IModelKey<TKey>
{
    TKey Id { get; init; }
}