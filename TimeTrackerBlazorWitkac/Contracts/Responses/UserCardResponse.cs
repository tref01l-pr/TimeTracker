using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Contracts.Responses;

public class UserCardResponse
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int CompanyId { get; set; }
    public string? UserDeletedId { get; set; } 
    public string Number { get; set; }
    public string Name { get; set; }
    public CardType CardType { get; set; }
    public bool IsActive { get; set; }
    public DateOnly CreatedAt { get; set; }
    public DateOnly? DeletedAt { get; set; }
    public CompanyResponse Company { get; set; }
}