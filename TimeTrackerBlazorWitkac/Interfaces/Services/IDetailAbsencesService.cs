using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;
using TimeTrackerBlazorWitkac.Contracts.Responses;

namespace TimeTrackerBlazorWitkac.Interfaces.Services
{
    public interface IDetailAbsencesService
    {
        Task<Result> DeleteDate(WorkDayResponse model, DetailAbsencesModel models);
    }
}
