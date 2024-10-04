using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services;

public enum AbsencesServiceErrorMessages
{
    [Description("Użytkownik nie istnieje!")]
    UserNotFound,               

    [Description("Typ nieobecności nie istnieje!")]
    AbsenceTypeNotFound,        

    [Description("Nieobecność nie istnieje!")]
    AbsenceNotFound,            

    [Description("Id użytkownika jest inny!")]
    UserIdMismatch,             

    [Description("Nie udało się przełączyć statusu!")]
    ToggleStatusFailed,         

    [Description("Wykryto kolizję dat!")]
    DateCollisionDetected,
}