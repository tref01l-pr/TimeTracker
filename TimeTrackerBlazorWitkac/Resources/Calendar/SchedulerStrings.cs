using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.Resources.Calendar;

public enum SchedulerTitle
{
    [Description("Urlop")]
    AbsenceTitle,
    [Description("Szczegóły")]
    DetailTitle,

}
public enum SchedulerParameters
{
    [Description("WorkDay")]
    WorkDayParameter,
    [Description("role")]
    RoleParameter,
    [Description("scheduler")]
    SchedulerParameter,
    [Description("models")]
    ModelParameter,
    [Description("parameters")]
    Parameters,
}

