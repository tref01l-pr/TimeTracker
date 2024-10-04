using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.QueryTags;

public enum UserQueryTags
{
    [Description("Check if user exists by email.")]
    IsExistsByEmail,

    [Description("Check if user exists by ID.")]
    IsExistsById,

    [Description("Get user by email.")]
    GetByEmail
}