using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;

public enum HolidayErrorMessages
{
    [Description("Nazwa święta nie może być pusta ani składać się wyłącznie ze spacji.")]
    InvalidName,

    [Description("Nazwa święta nie może być dłuższa niż 128 znaków.")]
    NameTooLong,

    [Description("Lokalna nazwa święta nie może być pusta ani składać się wyłącznie ze spacji.")]
    InvalidLocalName,

    [Description("Lokalna nazwa święta nie może być dłuższa niż 128 znaków.")]
    LocalNameTooLong,

    [Description("Data zakończenia święta nie może być wcześniejsza lub równa dacie rozpoczęcia.")]
    InvalidDateRange,

    [Description("Opis święta nie może być dłuższy niż 360 znaków.")]
    DescriptionTooLong
}