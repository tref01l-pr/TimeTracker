using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Repositories;

public enum HolidaysRepositoryErrorMessages
{
    [Description("Święto już istnieje.")]
    HolidayAlreadyExists,

    [Description("Święto nie zostało znalezione.")]
    HolidayNotFound,

    [Description("Usunięcie święta nie powiodło się.")]
    DeletionFailed
}