namespace TimeTrackerBlazorWitkac.Contracts.Responses;

public class UserDetailsResponse
{
    public string Id { get; set; }

    public string Email { get; set; }

    public string UserName { get; set; }
    public string Role { get; set; }
    public List<UserCardResponse> UserCards { get; set; }
}