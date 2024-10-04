using System.Text.RegularExpressions;

namespace TimeTrackerBlazorWitkac.Helpers;

public static class HexColorValidator
{
    public static bool IsValidHexColor(string color)
    {
        return Regex.IsMatch(color, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
    }

    public static bool IsValidRgbaColor(string color)
    {
        return Regex.IsMatch(color, @"^rgba\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3}),\s*(0|1|0\.\d+)\)$");
    }
}