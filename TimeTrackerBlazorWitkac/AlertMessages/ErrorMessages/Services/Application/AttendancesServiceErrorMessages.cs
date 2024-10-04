using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services.Application;

public enum AttendancesServiceErrorMessages
{
    [Description("Karta użytkownika nie została znaleziona w systemie.")]
    UserCardNotFound,

    [Description("Użytkownik nie został znaleziony w systemie.")]
    UserNotFound,

    [Description("Rekord obecności nie został znaleziony.")]
    AttendanceNotFound,

    [Description("Daty obecności kolidują z istniejącymi rekordami.")]
    AttendanceCollision,

    [Description("Wykryto dziwną aktywność w rekordzie obecności.")]
    StrangeActivityDetected,

    [Description("Dane wejściowe są nieprawidłowe.")]
    InvalidInputData,

    [Description("Nie udało się utworzyć rekordu obecności.")]
    AttendanceCreationFailed,

    [Description("Nie udało się zaktualizować rekordu obecności.")]
    AttendanceUpdateFailed,
    
    [Description("Nie można użyć tej metody, spróbuj użyć UpdateOrResolveAsync.")]
    AttendanceInvalidMethodUpdate,

    [Description("Identyfikator administratora nie został znaleziony w systemie.")]
    AdminIdNotFound
}