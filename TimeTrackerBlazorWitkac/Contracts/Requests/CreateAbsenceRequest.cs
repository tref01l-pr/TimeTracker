namespace TimeTrackerBlazorWitkac.Contracts.Requests;

public class CreateAbsenceRequest
{
    public string UserId { get; set; }
    public int AbsenceTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public int StartHour { get; set; }
    public int StartMinute { get; set; }
    public DateOnly EndDate { get; set; }
    public int EndHour { get; set; }
    public int EndMinute { get; set; }
    public bool IsFullDate { get; set; }
    public string? Reason { get; set; }
}