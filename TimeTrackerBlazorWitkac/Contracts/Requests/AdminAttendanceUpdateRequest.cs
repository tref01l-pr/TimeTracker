using System.ComponentModel.DataAnnotations;

namespace TimeTrackerBlazorWitkac.Contracts.Requests;

public class AdminAttendanceUpdateRequest : AdminAttendanceCreateRequest
{
    public string AdminId { get; set; }
    public int AttendanceId { get; set; }
}