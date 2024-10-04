using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services.Application;

public enum UserCardsServiceErrorMessages
{
    [Description("Karta użytkownika nie została znaleziona.")]
    UserCardNotFound,

    [Description("Użytkownik nie został znaleziony.")]
    UserNotFound,

    [Description("Użytkownik nie ma uprawnień administratora.")]
    UserNotAdmin,

    [Description("Karta użytkownika z podanym numerem już istnieje.")]
    UserCardNumberAlreadyExists,
}