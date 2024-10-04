using AutoMapper;
using BlazorBootstrap;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Components;
using Radzen;
using TimeTrackerBlazorWitkac.AlertMessages.SuccessMessages;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Data.Repositories;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;

namespace TimeTrackerBlazorWitkac.Service;

/// <summary>
/// Service for editing absences, responsible for confirming, deleting, and closing the dialog.
/// </summary>
public class AbsenceStatusService : IAbsenceStatusService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;
    private readonly DialogService _dialogService;
    private readonly IAbsencesService _absencesService;
    private readonly ToastService _notificationService;
    private readonly IToastNotificationService _toastNotificationService;

    /// <summary>
    /// Constructor with dependency injection for repositories, notifications, and dialog management.
    /// </summary>
    public AbsenceStatusService(
        IUsersRepository usersRepository,
        IMapper mapper,
        DialogService dialogService,
        IAbsencesService absencesService,
        ToastService notificationService,
        IToastNotificationService toastNotificationService)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
        _dialogService = dialogService;
        _absencesService = absencesService;
        _notificationService = notificationService;
        _toastNotificationService = toastNotificationService;
    }

    /// <summary>
    /// Closes the absence editing dialog.
    /// </summary>
    /// <param name="model">The WorkDay model passed when closing the dialog.</param>
    public void OnClose(WorkDayResponse model)
    {
        _dialogService.Close(model);
    }

    /// <summary>
    /// Confirms the absence type (RW).
    /// </summary>
    public async Task<Result<TProjectTo>> ConfirmTypeAbsence<TProjectTo>(int id) =>
        await UpdateConfirmationStatus<TProjectTo>(await _absencesService.ToggleStatusById<TProjectTo>(id, statusOfType: ConfirmationStatus.Confirmed));

    /// <summary>
    /// Confirms the absence dates (DW).
    /// </summary>
    public async Task<Result<TProjectTo>> ConfirmDatesAbsence<TProjectTo>(int id) =>
        await UpdateConfirmationStatus<TProjectTo>(await _absencesService.ToggleStatusById<TProjectTo>(id, statusOfDates: ConfirmationStatus.Confirmed));


    /// <summary>
    /// Updates the overall confirmation status if both type and date statuses are confirmed.
    /// </summary>
    /// <param name="updateResult"></param>
    private async Task<Result<TProjectTo>> UpdateConfirmationStatus<TProjectTo>(Result<TProjectTo> updateResult)
    {

        if (updateResult.IsFailure)
        {
            _toastNotificationService.ShowError(updateResult.Error);
            return updateResult;
        }
        
        _toastNotificationService.ShowSuccess(AbsenceSuccessMessages.UpdateSuccess.GetDescription());
        return updateResult;
    }

    /// <summary>
    /// Deletes the absence entry from the database.
    /// </summary>
    /// <param name="workDayResponse">The WorkDay model to be deleted.</param>
    public async Task<Result> DeleteDate(WorkDayResponse workDayResponse)
    {
        var isDeleted = await _absencesService.DeleteByIdAsync(workDayResponse.Id);

        if (isDeleted.IsFailure)
        {
            _toastNotificationService.ShowError(isDeleted.Error);
        }
        else
        {
            _toastNotificationService.ShowSuccess(AbsenceSuccessMessages.DeleteSuccess.GetDescription());
        }

        return isDeleted;
    }
}
