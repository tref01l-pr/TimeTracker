using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services.Application;

public enum AuthorizationServiceErrorMessages
{
    [Description("Użytkownik nie istnieje!")]
    UserNotFoundError,
    
    [Description("Kod nie istnieje!")]
    CodeNotFoundError,

    [Description("Adres e-mail jest już w użyciu!")]
    EmailAlreadyInUseError,
    
    [Description("Nie udało się potwierdzić adresu e-mail.")]
    EmailConfirmationFailedError,
    
    [Description("Usunięcie tokenu nie powiodło się.")]
    ConfirmationTokenDeletionFailedError,
    
    [Description("Token wygasł. Spróbuj ponownie wysłać token!")]
    ConfirmationTokenExpiredError
}