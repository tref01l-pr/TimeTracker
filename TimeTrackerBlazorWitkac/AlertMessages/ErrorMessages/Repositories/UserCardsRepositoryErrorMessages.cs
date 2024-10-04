using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Repositories;

public enum UserCardsRepositoryErrorMessages
{
    [Description("Karta użytkownika nie została znaleziona w systemie.")]
    UserCardNotFound,

    [Description("Nie udało się usunąć karty użytkownika.")]
    UserCardDeletionFailed,

    [Description("Nie można zmienić numeru karty użytkownika.")]
    CannotChangeNumber,

    [Description("Coś poszło nie tak podczas zapisywania zmian w UserCardsRepository.")]
    SaveChangesFailed
}