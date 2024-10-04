using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Repositories;

public enum RolesRepositoryErrorMessages
{
    [Description("The role already exists.")]
    RoleAlreadyExists,

    [Description("The role was not found.")]
    RoleNotFound,

    [Description("Something went wrong during role creation.")]
    CreationFailed
}