using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;

public enum AbsenceTypeErrorMessages
{
    [Description("Nazwa typu nieobecności nie może być pusta ani składać się wyłącznie z spacji.")]
    NameIsNullOrWhiteSpace,

    [Description("Nazwa typu nieobecności nie może być dłuższa niż 128 znaków.")]
    NameTooLong,

    [Description("Ikona typu nieobecności jest nieprawidłowa.")]
    IconIsInvalid,

    [Description("Opis typu nieobecności nie może być pusty ani składać się wyłącznie z spacji.")]
    DescriptionIsNullOrWhiteSpace,

    [Description("Opis typu nieobecności nie może być dłuższy niż 360 znaków.")]
    DescriptionTooLong,

    [Description("Kolor typu nieobecności nie może być pusty ani składać się wyłącznie z spacji.")]
    ColorIsNullOrWhiteSpace,

    [Description("Kolor typu nieobecności nie może być dłuższy niż 9 znaków.")]
    ColorTooLong,

    [Description("Kolor typu nieobecności nie jest prawidłowym kolorem hex.")]
    InvalidColorFormat
}