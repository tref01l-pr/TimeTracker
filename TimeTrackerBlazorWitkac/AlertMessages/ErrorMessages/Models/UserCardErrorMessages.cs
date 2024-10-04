using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;

public enum UserCardErrorMessages
{
    [Description("Identyfikator użytkownika nie może być pusty ani składać się wyłącznie ze spacji.")]
    InvalidUserId,

    [Description("Identyfikator firmy nie może być mniejszy lub równy 0.")]
    InvalidCompanyId,

    [Description("Numer karty nie może być pusty ani składać się wyłącznie ze spacji.")]
    InvalidCardNumber,

    [Description("Nazwa karty nie może być dłuższa niż 128 znaków.")]
    NameTooLong
}