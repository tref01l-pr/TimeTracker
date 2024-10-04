using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.SuccessMessages;

public enum AbsenceTypeSuccessMessages
{
    [Description("Typ nieobecności został pomyślnie utworzony.")]
    CreateSuccess,

    [Description("Typ nieobecności został pomyślnie zaktualizowany.")]
    UpdateSuccess,

    [Description("Typ nieobecności został pomyślnie usunięty.")]
    DeleteSuccess
}