using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;
using TimeTrackerBlazorWitkac.Contracts.Responses;

namespace TimeTrackerBlazorWitkac.Interfaces.Services
{
    public interface IModalAbsencesService
    {
        void GetInitialize(AbsenceFormParameters formParameters, WorkDayResponse model);
        Task GetParametersSetAsync(AbsenceFormParameters formParameters);
        Task<Result> HandleSubmit(WorkDayResponse model, AbsenceFormParameters formParameters);
    }
}
