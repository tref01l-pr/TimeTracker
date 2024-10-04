using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Interfaces;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;
using TimeTrackerBlazorWitkac.Helpers;

namespace TimeTrackerBlazorWitkac.Data.Models;

// The UserCard record class represents a user's card.
// It provides a structured way to work with user card data in the application.
public record UserCard : IModelKey<int>
{
    // Maximum length for the card's name.
    public const int MaxNameLength = 128;

    // Private constructor, used for creating instances via the Create method.
    // This ensures that all required validations and defaults are applied.
    protected UserCard(
        int id,
        string userId,
        int companyId,
        string? userDeletedId,
        string number,
        string name,
        CardType cardType,
        bool isActive,
        DateOnly createdAt,
        DateOnly? deletedAt)
    {
        Id = id;
        UserId = userId;
        CompanyId = companyId;
        UserDeletedId = userDeletedId;
        Number = number;
        Name = name;
        CardType = cardType;
        IsActive = isActive;
        CreatedAt = createdAt;
        DeletedAt = deletedAt;
    }

    public int Id { get; init; }
    public string UserId { get; }
    public int CompanyId { get; }
    public string? UserDeletedId { get; }
    public string Number { get; }
    public string? Name { get; }
    public CardType CardType { get; }
    public bool IsActive { get; init; }
    public DateOnly CreatedAt { get; }
    public DateOnly? DeletedAt { get; }
    
    public static UserCardBuilder Builder() => new UserCardBuilder();

    public class UserCardBuilder
    {
        private int _id;
        private string _userId = "";
        private int _companyId;
        private string? _userDeletedId;
        private string _number = "";
        private string? _name;
        private CardType _cardType;
        private bool _isActive;
        private DateOnly _createdAt;
        private DateOnly? _deletedAt;
        
        public UserCardBuilder SetId(int id)
        {
            _id = id;
            return this;
        }

        public UserCardBuilder SetUserId(string userId)
        {
            _userId = userId;
            return this;
        }

        public UserCardBuilder SetCompanyId(int companyId)
        {
            _companyId = companyId;
            return this;
        }

        public UserCardBuilder SetUserDeletedId(string? userDeletedId)
        {
            _userDeletedId = userDeletedId;
            return this;
        }

        public UserCardBuilder SetNumber(string number)
        {
            _number = number;
            return this;
        }

        public UserCardBuilder SetName(string? name)
        {
            _name = name;
            return this;
        }

        public UserCardBuilder SetCardType(CardType cardType)
        {
            _cardType = cardType;
            return this;
        }

        public UserCardBuilder SetIsActive(bool isActive)
        {
            _isActive = isActive;
            return this;
        }

        public UserCardBuilder SetCreatedAt(DateOnly createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public UserCardBuilder SetDeletedAt(DateOnly? deletedAt)
        {
            _deletedAt = deletedAt;
            return this;
        }
        
        public Result<UserCard> Build()
        {
            Result userCardValidationResult = ValidateUserCardData(_userId, _companyId, _number, _cardType, ref _name);

            if (userCardValidationResult.IsFailure)
            {
                return Result.Failure<UserCard>(userCardValidationResult.Error);
            }

            return new UserCard(
                _id,
                _userId,
                _companyId,
                null,
                _number,
                _name!,
                _cardType,
                true,
                DateOnly.FromDateTime(DateTime.Now),
                null
            );
        }
    }

    private static Result ValidateUserCardData(string userId, int companyId, string number, CardType cardType,
        ref string? name)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure<UserCard>(UserCardErrorMessages.InvalidUserId.GetDescription());
        }

        if (companyId <= 0)
        {
            return Result.Failure<UserCard>(UserCardErrorMessages.InvalidCompanyId.GetDescription());
        }

        if (string.IsNullOrWhiteSpace(number))
        {
            return Result.Failure<UserCard>(UserCardErrorMessages.InvalidCardNumber.GetDescription());
        }
        
        if (string.IsNullOrWhiteSpace(name))
        {
            name = $"{userId}-{cardType.ToString()}-{number}";
        }
        else if (name.Length >= MaxNameLength)
        {
            return Result.Failure<UserCard>(UserCardErrorMessages.NameTooLong.GetDescription());
        }

        return Result.Success();
    }
}