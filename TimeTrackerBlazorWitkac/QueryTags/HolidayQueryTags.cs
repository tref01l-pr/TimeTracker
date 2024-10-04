using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.QueryTags;

public enum HolidayQueryTags
{
    [Description("Get all holidays")]
    GetAllHolidays,

    [Description("Get holidays by year and month")]
    GetHolidaysByYearMonth,

    [Description("Get holiday by date with summary")]
    GetHolidayByDateWithSummary,
    
    [Description("Get holidays by dates")]
    GetHolidaysByDates
}