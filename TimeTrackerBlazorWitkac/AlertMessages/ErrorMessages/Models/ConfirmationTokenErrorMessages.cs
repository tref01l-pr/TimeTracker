using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;

public enum ConfirmationTokenErrorMessages
{
    [Description("Identyfikator musi być większy lub równy 0.")]
    InvalidId,

    [Description("Identyfikator użytkownika nie może być pusty ani składać się wyłącznie ze spacji.")]
    InvalidUserId,

    [Description("Token nie może być pusty ani składać się wyłącznie ze spacji.")]
    InvalidToken,

    [Description("Czas wygaśnięcia musi być większy niż 0.")]
    InvalidTimeForExpiration,

    [Description("Nieprawidłowy typ potwierdzenia.")]
    InvalidConfirmationType
}