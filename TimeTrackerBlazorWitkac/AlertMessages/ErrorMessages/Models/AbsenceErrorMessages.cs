using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;

public enum AbsenceErrorMessages
{
    [Description("Identyfikator użytkownika nie może być pusty ani składać się wyłącznie z spacji.")]
    InvalidUserId,

    [Description("Identyfikator typu nieobecności musi być większy od 0.")]
    InvalidAbsenceTypeId,

    [Description("Data początkowa nie może być większa lub równa dacie końcowej.")]
    InvalidDateRangeForFullDayAbsence,

    [Description("Data początkowa musi być równa dacie końcowej dla niepełnodniowych nieobecności.")]
    StartDateMustEqualEndDateForNonFullDayAbsence,

    [Description("Godzina i minuta muszą być w prawidłowym zakresie.")]
    InvalidTimeRange,

    [Description("Godzina rozpoczęcia i minuta rozpoczęcia muszą być mniejsze niż godzina zakończenia i minuta zakończenia.")]
    StartTimeMustBeLessThanEndTime,

    [Description("Powód nie może być dłuższy niż 360 znaków.")]
    ReasonTooLong,
}