using Microsoft.JSInterop;
using TimeTrackerBlazorWitkac.Interfaces.Services;

namespace TimeTrackerBlazorWitkac.Service;
/// <summary>
/// Service for Copy Text
/// </summary>
public class ClipboardService : IClipboardService
{
    private readonly IJSRuntime _jsRuntime;
    public ClipboardService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Using JS attributes to copy text 
    /// </summary>
    /// <param name="text">Copied text</param>
    /// <returns>Returns the JS attribute and invocation method, as well as the text</returns>
    public ValueTask GetClipboardAsync(string text)
    {
        return _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}

