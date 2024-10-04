using System.ComponentModel;

namespace TimeTrackerBlazorWitkac.QueryTags;

public enum UserCardQueryTags
{
    [Description("Get all user cards")]
    GetAllUserCards,

    [Description("Get UserCard by number")]
    GetUserCardByNumber,

    [Description("Get UserCard by user id")]
    GetUserCardsByUserId,
    
    [Description("Get by number to see is user card exist")]
    IsUserCardExistByNumber
}