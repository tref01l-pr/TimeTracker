using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using TimeTrackerBlazorWitkac.Data.Entities;

namespace TimeTrackerBlazorWitkac.Service
{
    public class ClaimsFactory : IClaimsTransformation
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<RoleEntity> _roleManager;

        public ClaimsFactory(UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager, IOptions<IdentityOptions> options)

        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user != null)
            {
                var token = await _userManager.GetAuthenticationTokenAsync(user, "Default", "Token");
                if (!string.IsNullOrEmpty(token))
                {
                    var identity = (ClaimsIdentity)principal.Identity;
                    if (!identity.HasClaim(c => c.Type == "Token"))
                    {
                        identity.AddClaim(new Claim("Token", token));
                    }
                }
            }
            return principal;
        }
    }
}
