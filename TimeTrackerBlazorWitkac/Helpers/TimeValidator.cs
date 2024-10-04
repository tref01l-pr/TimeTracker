using CSharpFunctionalExtensions;

namespace TimeTrackerBlazorWitkac.Helpers;

public static class TimeValidator
{
    public static bool ValidateTime(int hour, int minute, out TimeSpan timeSpan)
    {
        timeSpan = new TimeSpan(hour, minute, 0);
        
        if (timeSpan.Hours != hour || timeSpan.Minutes != minute)
        {
            return false;
        }

        if (timeSpan < TimeSpan.Zero || timeSpan >= TimeSpan.FromHours(24))
        {
            return false;
        }
        
        return true;
    }
}