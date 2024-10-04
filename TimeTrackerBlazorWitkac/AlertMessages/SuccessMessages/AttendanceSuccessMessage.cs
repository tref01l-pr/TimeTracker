using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.SuccessMessages;

public enum AttendanceSuccessMessage
{
    [Description("Obecność została pomyślnie utworzona.")]
    CreateSuccess,

    [Description("Obecność została pomyślnie zaktualizowana.")]
    UpdateSuccess,

    [Description("Obecność została pomyślnie usunięta.")]
    DeleteSuccess
}