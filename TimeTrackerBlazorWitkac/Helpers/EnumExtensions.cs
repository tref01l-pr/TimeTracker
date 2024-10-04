using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.Helpers;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        try
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                throw new Exception();

            var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            if (attribute == null)
                throw new Exception();

            return attribute.Description;
        }
        catch (Exception e)
        {
            return value.ToString();
        }
    }
}