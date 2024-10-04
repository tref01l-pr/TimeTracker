using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Repositories;

public enum AbsencesRepositoryErrorMessages
{
    [Description("Nie znaleziono nieobecności!")]
    AbsenceNotFound,

    [Description("Coś poszło nie tak podczas aktualizacji statusów!")]
    UpdatingFailed
}