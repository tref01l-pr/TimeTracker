using CSharpFunctionalExtensions;
using Radzen;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Interfaces.Services;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;

namespace TimeTrackerBlazorWitkac.Service;

public class DetailAbsencesService : IDetailAbsencesService
{
    private readonly DialogService _dialogService;
    private readonly IAbsencesService _absencesService;

    public DetailAbsencesService(DialogService dialogService, IAbsencesService absencesService)
    {
        _dialogService = dialogService;
        _absencesService = absencesService;
    }

    /// <summary>
    /// Closes the dialog with the given model.
    /// </summary>
    /// <param name="model">The model to return when closing the dialog.</param>
    private void CloseDialog(WorkDayResponse model)
    {
        _dialogService.Close(model);
    }

    /// <summary>
    /// Deletes the absence by its ID and handles the result.
    /// </summary>
    /// <param name="model">The work day response model.</param>
    /// <param name="models">The detail absences model containing the absence ID.</param>
    public async Task<Result> DeleteDate(WorkDayResponse model, DetailAbsencesModel models)
    {

        var isDeleted = await _absencesService.DeleteByIdAsync(models.Absence.Id);
        return isDeleted;
    }
}