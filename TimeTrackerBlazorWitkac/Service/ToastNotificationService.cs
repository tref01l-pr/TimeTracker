using BlazorBootstrap;
using TimeTrackerBlazorWitkac.Interfaces.Services;

namespace TimeTrackerBlazorWitkac.Service;

public class ToastNotificationService : IToastNotificationService
{
    private readonly List<ToastMessage> _messages = new();

    public event Action OnChange;

    public void ShowSuccess(string message) => ShowToast(ToastType.Success, "Result", $"{DateTime.Now}", message);
    public void ShowError(string message) => ShowToast(ToastType.Danger, "Result", $"{DateTime.Now}", message);
    public void ShowInfo(string message) => ShowToast(ToastType.Info, "Result", $"{DateTime.Now}", message);



    private void ShowToast(ToastType type, string title, string dateTime, string message)
    {
        var toastMessage = new ToastMessage
        {
            Type = type,
            Title = title,
            HelpText = dateTime,
            Message = message,
            AutoHide = true
        };

        _messages.Add(toastMessage);
        OnChange?.Invoke();

        Task.Delay(6000).ContinueWith(_ =>
        {
            _messages.Remove(toastMessage);
            OnChange?.Invoke();
        });
    }

    public List<ToastMessage> GetMessages() => _messages;
}