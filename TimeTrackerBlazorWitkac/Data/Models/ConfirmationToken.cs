using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces;

namespace TimeTrackerBlazorWitkac.Data.Models;

public record ConfirmationToken : IModelKey<int>
{
    public const int MinutesForExpiration = 120;
    
    protected ConfirmationToken(
        int id,
        string userId,
        string token,
        DateTime expirationTime,
        ConfirmationTypes confirmationType)
    {
        Id = id;
        UserId = userId;
        Token = token;
        Expiration = expirationTime;
        ConfirmationType = confirmationType;
    }
    
    public int Id { get; init; }
    public string UserId { get; } 
    public string Token { get; } 
    public DateTime Expiration { get; } 
    public ConfirmationTypes ConfirmationType { get; }

    public static ConfirmationTokenBuilder Builder() => new ConfirmationTokenBuilder();
    
    public class ConfirmationTokenBuilder
    {
        private int _id;
        private string _userId;
        private string _token;
        private int _timeForExpiration = MinutesForExpiration;
        private ConfirmationTypes _confirmationType;
        
        public ConfirmationTokenBuilder SetId(int id)
        {
            _id = id;
            return this;
        }

        public ConfirmationTokenBuilder SetUserId(string userId)
        {
            _userId = userId;
            return this;
        }

        public ConfirmationTokenBuilder SetToken(string token)
        {
            _token = token;
            return this;
        }

        public ConfirmationTokenBuilder SetTimeForExpiration(int timeForExpiration)
        {
            _timeForExpiration = timeForExpiration;
            return this;
        }

        public ConfirmationTokenBuilder SetConfirmationType(ConfirmationTypes confirmationType)
        {
            _confirmationType = confirmationType;
            return this;
        }

        public Result<ConfirmationToken> Build()
        {
            Result confirmationTokenValidationResult =
                ValidateConfirmationTokenData(_id, _userId, _token, _timeForExpiration, _confirmationType);

            if (confirmationTokenValidationResult.IsFailure)
            {
                return Result.Failure<ConfirmationToken>(confirmationTokenValidationResult.Error);
            }

            return new ConfirmationToken(
                _id,
                _userId,
                _token,
                DateTime.Now.AddMinutes(_timeForExpiration),
                _confirmationType);
        }

        private Result ValidateConfirmationTokenData(int id, string userId, string token, int timeForExpiration, ConfirmationTypes confirmationType)
        {
            if (id < 0)
            {
                return Result.Failure(ConfirmationTokenErrorMessages.InvalidId.GetDescription());
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure(ConfirmationTokenErrorMessages.InvalidUserId.GetDescription());
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return Result.Failure(ConfirmationTokenErrorMessages.InvalidToken.GetDescription());
            }

            if (timeForExpiration <= 0)
            {
                return Result.Failure(ConfirmationTokenErrorMessages.InvalidTimeForExpiration.GetDescription());
            }

            if (!Enum.IsDefined(typeof(ConfirmationTypes), confirmationType))
            {
                return Result.Failure(ConfirmationTokenErrorMessages.InvalidConfirmationType.GetDescription());
            }
            
            return Result.Success();
        }
    }
}