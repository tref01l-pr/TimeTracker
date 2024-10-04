using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.SuccessMessages;

public enum CompanySuccessMessages
{
    [Description("Firma została pomyślnie utworzona.")]
    CreateSuccess,

    [Description("Firma została pomyślnie zaktualizowana.")]
    UpdateSuccess,

    [Description("Firma została pomyślnie usunięta.")]
    DeleteSuccess
}