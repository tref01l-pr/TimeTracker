using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;

public enum AttendanceErrorMessages
{
    [Description("Identyfikator karty użytkownika musi być większy od 0.")]
    InvalidUserCardId,

    [Description("Identyfikator użytkownika nie może być pusty ani składać się wyłącznie z spacji.")]
    InvalidUserId,

    [Description("Identyfikator firmy musi być większy od 0.")]
    InvalidCompanyId,

    [Description("Nieprawidłowe wartości godziny rozpoczęcia.")]
    InvalidStartTime,

    [Description("Data zakończenia nie może być wcześniejsza lub równa dacie rozpoczęcia.")]
    InvalidEndDate,

    [Description("Godzina rozpoczęcia musi być wcześniejsza niż godzina zakończenia.")]
    StartTimeMustBeLessThanEndTime,
    
    [Description("Start time must be less than or equal to real time.")]
    StartTimeMustBeLessThanRealTime,
    
    [Description("End time must be less than or equal to end time.")]
    EndTimeMustBeLessThanRealTime,

    [Description("Powód podejrzanej aktywności przekracza maksymalną długość 360 znaków.")]
    StrangeActivityReasonTooLong,

    [Description("ResolvedAt i ResolvedById muszą mieć jednocześnie wartość lub być puste.")]
    InvalidResolvedAtAndBy,
}