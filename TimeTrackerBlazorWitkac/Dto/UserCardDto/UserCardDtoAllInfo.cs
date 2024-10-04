using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Dto.UserCardDto;

public record UserCardDtoAllInfo : UserCard
{
    public UserCardDtoAllInfo(
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
    : base(id, userId, companyId, userDeletedId, number, name, cardType, isActive, createdAt, deletedAt)
    { }

    public Company? Company { get; init; }
    public User? User { get; init; }
    public User? DeletedUser { get; init; }

   
}