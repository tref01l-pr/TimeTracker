using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;

public enum CompanyErrorMessages
{
    [Description("Nazwa firmy nie może być pusta ani składać się wyłącznie z spacji.")]
    InvalidName,

    [Description("Nazwa firmy nie może być dłuższa niż 128 znaków.")]
    NameTooLong,

    [Description("Data założenia firmy nie może być w przyszłości.")]
    InvalidDateOfFoundation
}