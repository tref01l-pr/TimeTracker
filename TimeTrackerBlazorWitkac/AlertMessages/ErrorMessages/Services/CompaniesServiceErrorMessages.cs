using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services;

public enum CompaniesServiceErrorMessages
{
    [Description("Firma nie została znaleziona!")]
    CompanyNotFound,

    [Description("Firma o tej nazwie już istnieje!")]
    CompanyNameExists,
}