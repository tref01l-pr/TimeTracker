namespace TimeTrackerBlazorWitkac.Contracts.Responses;

public class PageResponse<T>
{
    public IList<T> Items { get; }
    public int TotalCount { get; }

    public PageResponse(List<T> items, int totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }
}