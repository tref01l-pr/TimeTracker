namespace TimeTrackerBlazorWitkac.Contracts.Requests;

public class UpdateHolidayRequest
{
    public string Summary { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Description { get; set; }
}