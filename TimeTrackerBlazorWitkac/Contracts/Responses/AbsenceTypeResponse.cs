using TimeTrackerBlazorWitkac.Data.Entities;

namespace TimeTrackerBlazorWitkac.Contracts.Responses;

public class AbsenceTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string AcceptedIconColor { get; set; }
    public string AcceptedBackgroundColor { get; set; }
    public string PendingIconColor { get; set; }
    public string PendingBackgroundColor { get; set; }
    public BlazorBootstrap.IconName Icon { get; set; }
}

