using BlazorBootstrap;

namespace TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;

public class NotificationMessageModel
{
    public string Title { get; set; }
    public string Message { get; set; }
    public ToastType ToastType { get; set; }
}

