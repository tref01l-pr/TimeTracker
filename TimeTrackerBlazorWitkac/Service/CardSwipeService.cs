using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Service.Application;

namespace TimeTrackerBlazorWitkac.Service
{
    public class CardSwipeService : ICardSwipeService
    {
        private readonly IUserCardsService _userCardsService;
        private readonly IAttendancesService _attendancesService;
        private readonly IJSRuntime _jsRuntime;
        private readonly IMapper _mapper;

        public CardSwipeService(IMapper mapper, IAttendancesService attendancesService, IUserCardsService userCardsService, IJSRuntime jsRuntime)
        {
            _mapper = mapper;
            _attendancesService = attendancesService;
            _userCardsService = userCardsService;
            _jsRuntime = jsRuntime;
        }

        public async Task SetParametersByAttendance(CardSwipeModel model, UserEntity user)
        {
            if (!IsValidUser(user, out var errorMessage))
            {
                model.ErrorMessage = errorMessage;
                return;
            }

            var cardResult = await _userCardsService.GetByUserAsync<UserCardResponse>(user.Id);

            if (cardResult.IsFailure)
            {
                model.ErrorMessage = cardResult.Error;
                return;
            }

            if (cardResult.Value.Count == 0)
            {
                model.ErrorMessage = "Error retrieving card number";
                return;
            }

            model.UserCards = cardResult.Value.ToList();
            MapUserCardToModel(model, cardResult.Value.FirstOrDefault()!);
            await UpdateAttendanceStatus(model);
        }

        public async Task<Result<Attendance>> GetCardPunchAsync(CardSwipeModel model, UserEntity user)
        {
            model.ErrorMessage = string.Empty;

            if (await IsInvalidClockInAttempt(model, user))
                return Result.Failure<Attendance>(model.ErrorMessage);

            var res = await _attendancesService.CardPunchAsync<Attendance>(model.UserCardNumber, DateTime.Now);
            if (!res.IsSuccess)
            {
                model.ErrorMessage = res.Error;
                return Result.Failure<Attendance>(res.Error);
            }

            return Result.Success(res.Value);
        }

        public async Task<Result<Attendance>> GetOpenAttendance(CardSwipeModel model, UserEntity user)
        {
            model.ErrorMessage = string.Empty;

            if (await IsInvalidClockInAttempt(model, user))
                return Result.Failure<Attendance>(model.ErrorMessage);

            var clockInResult = await _attendancesService.CardPunchAsync<Attendance>(model.UserCardNumber, DateTime.Now);

            if (clockInResult.IsFailure)
            {
                model.ErrorMessage = clockInResult.Error;
                return clockInResult;
            }

            return Result.Success(clockInResult.Value);
        }

        public async Task<Result<Attendance>> GetCloseAttendance(CardSwipeModel model, UserEntity user)
        {
            model.ErrorMessage = string.Empty;
            if (model.Attendance == null || !model.IsClockedIn)
                return Result.Failure<Attendance>(model.ErrorMessage);

            var clockOutResult = await _attendancesService.CardPunchAsync<Attendance>(model.UserCardNumber, DateTime.Now);
            if (clockOutResult.IsFailure)
            {
                model.ErrorMessage = clockOutResult.Error;
                return Result.Failure<Attendance>(model.ErrorMessage);
            }

            return Result.Success(clockOutResult.Value);
        }

        public async Task GetCardChange(ChangeEventArgs e, CardSwipeModel model)
        {
            var selectedCardNumber = e.Value.ToString();

            if (!string.IsNullOrEmpty(selectedCardNumber))
            {
                var selectedCard = model.UserCards.FirstOrDefault(card => card.Number == selectedCardNumber);
                if (selectedCard != null)
                {
                    MapUserCardToModel(model, selectedCard);
                    await UpdateAttendanceStatus(model);
                }

               
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "selectedCardNumber", selectedCardNumber);
            }
        }


        private bool IsValidUser(UserEntity user, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                errorMessage = "Użytkownik nie jest ustawiony.";
                return false;
            }

            return true;
        }

        private async Task<bool> IsInvalidClockInAttempt(CardSwipeModel model, UserEntity user)
        {
            if (model.IsClockedIn)
            {
                model.ErrorMessage = "Użytkownik jest już zalogowany.";
                return true;
            }

            var result = await _userCardsService.GetByNumberAsync<UserCard>(model.UserCardNumber);

            switch (result)
            {
                case { IsFailure: true }:
                    model.ErrorMessage = result.Error;
                    return true;
                case { Value: null }:
                    model.ErrorMessage = "Nieprawidłowy numer karty. Spróbuj ponownie.";
                    return true;
                default:
                    return false;
            }
        }

        public void MapUserCardToModel(CardSwipeModel model, UserCardResponse userCard)
        {
            model.UserCardNumber = userCard.Number ?? "";
            model.CompanyId = userCard.CompanyId;
            model.Company = userCard.Company.Name;
            model.UserCardId = userCard.Id;
        }

        public async Task UpdateAttendanceStatus(CardSwipeModel model)
        {
            var lastAttendanceResult = await _attendancesService.GetLastByCardIdAsync<Attendance>(model.UserCardId);

            if (lastAttendanceResult.IsFailure)
            {
                model.ErrorMessage = lastAttendanceResult.Error;
                return;
            }

            if (lastAttendanceResult.Value == null || lastAttendanceResult.Value!.EndDate != null)
            {
                model.IsClockedIn = false;
            }
            else
            {
                model.IsClockedIn = true;
                model.Attendance = lastAttendanceResult.Value;
            }
        }

        private UserCard CreateUserCard(CardSwipeModel model, UserEntity user)
        {
            return _mapper.Map<UserCard>(new UserCardResponse
            {
                Id = model.UserCardId,
                UserId = user.Id,
                Number = model.UserCardNumber,
                CompanyId = model.CompanyId
            });
        }


    }
}