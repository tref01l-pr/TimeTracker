using System.Net.Mail;
using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Interfaces;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;
using TimeTrackerBlazorWitkac.Helpers;

namespace TimeTrackerBlazorWitkac.Data.Models;
//didn't make implementation for it yet
public record User : IModelKey<string>
{
    protected User(string id, string email, string userName)
    {
        Id = id;
        Email = email;
        UserName = userName;
    }
    
    public string Id { get; init; }

    public string Email { get; }

    public string UserName { get; }

    public static UserBuilder Builder() => new UserBuilder();

    public class UserBuilder
    {
        private string _id = string.Empty;
        private string _email = default!;
        private string _userName = default!;
        
        public UserBuilder SetId(string id)
        {
            _id = id;
            return this;
        }
        
        public UserBuilder SetEmail(string email)
        {
            _email = email;
            return this;
        }
        
        public UserBuilder SetUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        public Result<User> Build()
        {
            var validationResult = ValidateUserData(_email, _userName);
            
            if (validationResult.IsFailure)
            {
                return Result.Failure<User>(validationResult.Error);
            }

            return Result.Success(new User(_id, _email, _userName));
        }

        private static Result ValidateUserData(string email, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return Result.Failure<User>(UserErrorMessages.InvalidUserName.GetDescription());
            }
            
            if (!IsValidEmail(email))
            {
                return Result.Failure<User>(UserErrorMessages.InvalidEmail.GetDescription());
            }

            return Result.Success();
        }
    }
    
    private static bool IsValidEmail(string email)
    {
        try
        {
            var address = new MailAddress(email);
            return address.Address == email;
        }
        catch
        {
            return false;
        }
    }
}