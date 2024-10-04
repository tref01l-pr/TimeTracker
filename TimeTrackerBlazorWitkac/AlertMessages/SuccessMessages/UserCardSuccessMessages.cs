using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.SuccessMessages;

public enum UserCardSuccessMessages
{
    [Description("Karta użytkownika została pomyślnie utworzona.")]
    CreateSuccess,

    [Description("Karta użytkownika została pomyślnie zaktualizowana.")]
    UpdateSuccess,

    [Description("Status Karty użytkownika IsActive został zmieniony.")]
    ToggleIsActiveSuccess,

    [Description("Karta użytkownika została pomyślnie usunięta.")]
    DeleteSuccess
}