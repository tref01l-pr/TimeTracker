namespace TimeTrackerBlazorWitkac.Resources.User;

public class UserSettings
{
    public int UserId { get; set; }
    public string PendingBackgroundColorByUser { get; set; } = Constants.DefaultPendingBackgroundColorByUser;
    public string AcceptedBackgroundColorByUser { get; set; } = Constants.DefaultAcceptedBackgroundColorByUser;
}

