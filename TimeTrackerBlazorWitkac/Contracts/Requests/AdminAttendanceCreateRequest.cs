using System.ComponentModel.DataAnnotations;

namespace TimeTrackerBlazorWitkac.Contracts.Requests;

public class AdminAttendanceCreateRequest
{
    public int UserCardId { get; set; }
    public DateOnly StartDate { get; set; }
    [Range(0,23)]
    public int StartHour { get; set; }
    [Range(0,59)]
    public int StartMinute { get; set; }
    public DateOnly? EndDate { get; set; }
    [Range(0,23)]
    public int EndHour { get; set; }
    [Range(0,59)]
    public int EndMinute { get; set; }
}