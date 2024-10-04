using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.InputModels;

public enum CreateUserCardInputModelErrorMessages
{
    [Description("CardType nie został wybrany!")]
    CardTypeNotSelected,
    
    [Description("Firma nie została wybrana!")]
    CompanyNotSelected
}