using BlazorBootstrap;

namespace TimeTrackerBlazorWitkac.Interfaces.Services;

public interface IBaseToastNotificationService
{
    public void ShowSuccess(string message);
    public void ShowError(string message);
    public void ShowInfo(string message);
}
public interface IToastNotificationService : IBaseToastNotificationService
{
    public event Action OnChange;
    public List<ToastMessage> GetMessages();
}