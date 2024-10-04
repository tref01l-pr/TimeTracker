using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services;

public enum HolidaysServiceErrorMessages
{
    [Description("Święto już istnieje.")]
    HolidayAlreadyExists,

    [Description("Święto nie zostało znalezione.")]
    HolidayNotFound,
}