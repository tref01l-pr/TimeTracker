using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Repositories;

public enum ConfirmationTokensRepositoryErrorMessages
{
    [Description("Nie możesz używać metody aktualizacji dla tokenów potwierdzających.")]
    UpdateMethodNotAllowed,
}