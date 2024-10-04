using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;

public class CardSwipeModel
{
    public Attendance? Attendance { get; set; }
    public string UserCardNumber { get; set; } = default!;
    public string Company { get; set; } = default!;
    public int CompanyId { get; set; }
    public int UserCardId { get; set; }
    public IList<UserCardResponse>? UserCards { get; set; } = new List<UserCardResponse>(); 
    public bool IsClockedIn { get; set; }
    public string ErrorMessage { get; set; } = "";

}

