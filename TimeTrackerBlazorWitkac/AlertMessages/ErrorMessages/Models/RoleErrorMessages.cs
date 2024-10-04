using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;

public enum RoleErrorMessages
{
    [Description("Role name can't be null or whitespace.")]
    InvalidName,
    
    [Description("Role id is invalid.")]
    InvalidId
}