using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;

public enum UserErrorMessages
{
    [Description("Nazwa użytkownika nie może być pusta.")]
    InvalidUserName,

    [Description("Adres e-mail jest nieprawidłowy.")]
    InvalidEmail
}