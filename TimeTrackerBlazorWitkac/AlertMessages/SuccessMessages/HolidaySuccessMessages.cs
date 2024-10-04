using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.SuccessMessages;

public enum HolidaySuccessMessages
{
    [Description("Święto zostało pomyślnie utworzone.")]
    CreateSuccess,

    [Description("Święto zostało pomyślnie zaktualizowane.")]
    UpdateSuccess,

    [Description("Święto zostało pomyślnie usunięte.")]
    DeleteSuccess
}