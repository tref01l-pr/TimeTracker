using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.SuccessMessages;

public enum AbsenceSuccessMessages
{
    [Description("Nieobecność została pomyślnie utworzona.")]
    CreateSuccess,

    [Description("Nieobecność została pomyślnie zaktualizowana.")]
    UpdateSuccess,

    [Description("Nieobecność została pomyślnie usunięta.")]
    DeleteSuccess
}