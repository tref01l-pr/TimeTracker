using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Options;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

/// <summary>
/// Defines operations related to user management, including sign-in, user creation, and existence checks.
/// </summary>
public interface IUsersRepository : ICrudRepository<UserEntity, User, string>
{
    /// <summary>
    /// Signs in a user based on the provided user entity.
    /// </summary>
    /// <param name="userEntity">The user entity to sign in.</param>
    Task SignInUser(UserEntity userEntity);
    
    /// <summary>
    /// Creates a new user entity with the specified email.
    /// </summary>
    /// <param name="email">The email address for the new user.</param>
    /// <returns>A new instance of the user entity.</returns>
    UserEntity CreateUser(string email);
    
    /// <summary>
    /// Checks if a user with the specified email exists in the system.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    Task<bool> IsUserExistsByEmail(string email);
    
    /// <summary>
    /// Checks if a user with the specified email exists in the system.
    /// </summary>
    /// <param name="id"> Id to check.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    Task<bool> IsUserExistsById(string id);
    
    /// <summary>
    /// Redirects the user to the Microsoft login page.
    /// </summary>
    void RoutingToMsLogin();
    
    /// <summary>
    /// Signs in the user if they already exist in the system based on their email.
    /// </summary>
    /// <param name="email">The email address of the user to sign in.</param>
    Task SignInIfExists(string email);

    Task GenerateToken(string token, string email);
    Task AddClaimToUser(string email, string token);
    Task SignInUserClaims(UserEntity user);
    
    
    /// <summary>
    /// Checks if a user with the specified email exists in the system.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    Task<bool> IsExistsByEmailAsync(string email);

    /// <summary>
    /// Checks if a user with the specified email exists in the system.
    /// </summary>
    /// <param name="id"> Id to check.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    Task<bool> IsExistsByIdAsync(string id);

    Task<TProjectTo?> GetByEmailAsync<TProjectTo>(string? email);
    
    Task<Result<UserEntity>> CreateWithPasswordAsync(User model, string password);
    Task<Result<UserEntity>> CreateWithMsTokenAsync(User model);
}

