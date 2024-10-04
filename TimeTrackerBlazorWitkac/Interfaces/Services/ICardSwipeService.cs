using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Services;

public interface IBaseCardSwipeService
{
    Task<Result<Attendance>> GetCardPunchAsync(CardSwipeModel model, UserEntity user);
    Task<Result<Attendance>> GetOpenAttendance(CardSwipeModel model, UserEntity user);
    Task<Result<Attendance>> GetCloseAttendance(CardSwipeModel model, UserEntity user);
}
public interface ICardSwipeService : IBaseCardSwipeService
{
    Task SetParametersByAttendance(CardSwipeModel model, UserEntity user);
    void MapUserCardToModel(CardSwipeModel model, UserCardResponse userCard);
    Task GetCardChange(ChangeEventArgs e, CardSwipeModel model);
    Task UpdateAttendanceStatus(CardSwipeModel model);
}

