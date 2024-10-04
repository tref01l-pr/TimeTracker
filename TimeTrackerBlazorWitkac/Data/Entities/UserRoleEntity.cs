using Microsoft.AspNetCore.Identity;

namespace TimeTrackerBlazorWitkac.Data.Entities;

public class UserRoleEntity : IdentityUserRole<string>
{
    public virtual UserEntity User { get; set; }
    public virtual RoleEntity Role { get; set; }
}