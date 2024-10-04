using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.SuccessMessages;

public enum UserSuccessMessages
{
    [Description("Użytkownik został pomyślnie utworzony.")]
    CreateSuccess,

    [Description("Użytkownik został pomyślnie zaktualizowany.")]
    UpdateSuccess,

    [Description("Użytkownik został pomyślnie usunięty.")]
    DeleteSuccess
}