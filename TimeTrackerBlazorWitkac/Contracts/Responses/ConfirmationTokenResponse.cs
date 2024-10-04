namespace TimeTrackerBlazorWitkac.Contracts.Responses;

public class ConfirmationTokenResponse
{
    public int Id { get; set; }
    public string UserId { get; set; } 
    public string Token { get; set; } 
    public DateTime Expiration { get; set; } 
    public ConfirmationTypes ConfirmationType { get; set; }
}