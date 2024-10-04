using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.QueryTags;

public enum AttendanceQueryTags
{
    [Description("Get last attendance by card id.")]
    GetLastByCardId,

    [Description("Check for collision for startDate.")]
    CheckCollisionForStartDate,

    [Description("Check for collision for start and end dates.")]
    CheckCollisionForStartAndEndDates
}