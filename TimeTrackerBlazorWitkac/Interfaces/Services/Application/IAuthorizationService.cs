using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;

public interface IAuthorizationService
{
    Task<Result<UserEntity>> RegisterWithExternalTokenAsync(User user, string msId, string authProvider, string roleStr);
    Task<Result<UserEntity>> RegisterWithPasswordAsync(User user, string password, string roleStr, string? returnUrl = null);
    Task<Result> VerifyEmailConfirmationTokenAsync(string userId, string token);
    Task<Result> SignOut();
}