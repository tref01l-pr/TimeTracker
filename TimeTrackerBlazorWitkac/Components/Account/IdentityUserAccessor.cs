using Microsoft.AspNetCore.Identity;
using TimeTrackerBlazorWitkac.Data;
using TimeTrackerBlazorWitkac.Data.Entities;

namespace TimeTrackerBlazorWitkac.Components.Account
{
    internal sealed class IdentityUserAccessor(UserManager<UserEntity> userManager, IdentityRedirectManager redirectManager)
    {
        public async Task<UserEntity> GetRequiredUserAsync(HttpContext context)
        {
            UserEntity? user = await userManager.GetUserAsync(context.User);
            
            Console.WriteLine("I'm here))");
            if (user is null)
            {
                redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            }

            return user;
        }
    }
}
