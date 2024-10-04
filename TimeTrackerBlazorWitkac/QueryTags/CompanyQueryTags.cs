using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.QueryTags;

public enum CompanyQueryTags
{
    [Description("Get all Companies")]
    GetAllCompanies,

    [Description("Get Company by name")]
    GetCompanyByName,
}