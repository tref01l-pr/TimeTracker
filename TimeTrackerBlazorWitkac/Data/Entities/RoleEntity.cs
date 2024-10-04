using Microsoft.AspNetCore.Identity;
using TimeTrackerBlazorWitkac.Interfaces.Repository;

namespace TimeTrackerBlazorWitkac.Data.Entities;

public class RoleEntity : IdentityRole, IDbKey<string>
{
    public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
}