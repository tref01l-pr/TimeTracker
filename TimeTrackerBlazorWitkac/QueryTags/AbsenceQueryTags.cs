using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.QueryTags;

public enum AbsenceQueryTags
{
    [Description("Get all absence records")]
    GetAllAbsences,

    [Description("Get absence records by user ID")]
    GetAbsencesByUserId,
}