namespace TimeTrackerBlazorWitkac.Interfaces.Services;

public interface IClipboardService
{
    ValueTask GetClipboardAsync(string text);
}

