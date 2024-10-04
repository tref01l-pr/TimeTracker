using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Dto.UserCardDto;

namespace TimeTrackerBlazorWitkac.Dto.UserDto;

public record UserDtoUserCards : User
{
    public UserDtoUserCards(string id, string email, string userName) 
        : base(id, email, userName)
    { }
    
    public List<UserCardDtoCompany> UserCards { get; init; }
}