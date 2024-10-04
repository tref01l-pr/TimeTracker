using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Contracts.Requests;

public class CreateUserCardRequest
{
    public string UserId { get; set; }
    public int CompanyId { get; set; }
    public string Number { get; set; }
    public string? Name { get; set; }
    public CardType CardType { get; set; }
}