using TimeTrackerBlazorWitkac.Data.Entities;

namespace TimeTrackerBlazorWitkac.Contracts.Responses;

public class UserResponse
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string DateOfBirth { get; set; }
    public List<string> Roles { get; set; }
}