using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;

public class IconStatus : AbsenceTypeResponse
{
    public ConfirmationStatus StatusOfType { get; set; }
    public ConfirmationStatus StatusOfDates { get; set; }
    public ConfirmationStatus ConfirmationStatus { get; set; }

    public string Text { get; set; }    
    public BlazorBootstrap.IconName IconName { get; set; }
}