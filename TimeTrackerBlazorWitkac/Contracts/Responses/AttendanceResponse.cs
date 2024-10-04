
namespace TimeTrackerBlazorWitkac.Contracts.Responses;
public class AttendanceResponse
{
    public int Id { get; set; }
    public int UserCardId { get; set; }
    public string UserId { get; set; }
    public int CompanyId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int StartHour { get; set; }
    public int StartMinute { get; set; }
    public int EndHour { get; set; }
    public int EndMinute { get; set; }
    public UserCardResponse UserCard { get; set; }
}

