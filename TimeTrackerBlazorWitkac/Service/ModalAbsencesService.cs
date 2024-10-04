using AutoMapper;
using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.AlertMessages.SuccessMessages;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;

namespace TimeTrackerBlazorWitkac.Service
{
    /// <summary>
    /// Service to manage absence-related operations within a modal.
    /// </summary>
    public class ModalAbsencesService : IModalAbsencesService
    {
        private readonly IAbsenceTypesService _absenceTypesService;
        private readonly IAbsencesService _absencesService;
        private readonly IMapper _mapper;
        private readonly IToastNotificationService _toastNotificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModalAbsencesService"/> class.
        /// </summary>
        /// <param name="absencesService">The service for managing absences.</param>
        /// <param name="absenceTypesService"></param>
        /// <param name="mapper">The mapper for converting between models.</param>
        /// <param name="toastNotificationService"></param>
        public ModalAbsencesService(
            IAbsencesService absencesService,
            IAbsenceTypesService absenceTypesService,
            IMapper mapper,
            IToastNotificationService toastNotificationService)
        {
            _absencesService = absencesService;
            _absenceTypesService = absenceTypesService;
            _mapper = mapper;
            _toastNotificationService = toastNotificationService;
        }

        /// <summary>
        /// Initializes the form parameters from the model data.
        /// </summary>
        /// <param name="formParameters">The form parameters to initialize.</param>
        /// <param name="model">The model containing initial data.</param>
        public void GetInitialize(AbsenceFormParameters formParameters, WorkDayResponse model)
        {
            model.UserId = formParameters.User.Id;
            model.User = formParameters.User;
            model.IsFullDate = formParameters.IsFullDate;

            model.StartDate = formParameters.StartTime.Date
                .AddHours(formParameters.StartHour ?? 0)
                .AddMinutes(formParameters.StartMinute ?? 0);

            model.EndDate = formParameters.EndTime?.Date
                .AddHours(formParameters.EndHour ?? 0)
                .AddMinutes(formParameters.EndMinute ?? 0);
        }

        /// <summary>
        /// Asynchronously sets up the form parameters, including fetching necessary data.
        /// </summary>
        /// <param name="formParameters">The form parameters to set.</param>
        public async Task GetParametersSetAsync(AbsenceFormParameters formParameters)
        {
            formParameters.startHourSelector.Value = formParameters.StartHour ?? 0;
            formParameters.startMinuteSelector.Value = formParameters.StartMinute ?? 0;
            formParameters.endHourSelector.Value = formParameters.EndHour ?? 0;
            formParameters.endMinuteSelector.Value = formParameters.EndMinute ?? 0;

            var absenceTypesResult = await _absenceTypesService.GetAllAsync<AbsenceTypeResponse>();
            formParameters.types = absenceTypesResult.Value;
        }

        /// <summary>
        /// Handles the submission of the absence form.
        /// </summary>
        /// <param name="model">The model containing form data to be submitted.</param>
        public async Task<Result> HandleSubmit(WorkDayResponse model, AbsenceFormParameters formParameters)
        {
            if (model.StartDate.Date != model.EndDate?.Date)
            {
                formParameters.IsFullDate = true; 
            }
           
            model.StartDate = model.StartDate.Date
                .AddHours(formParameters.startHourSelector.Value)
                .AddMinutes(formParameters.startMinuteSelector.Value);

            model.EndDate = model.EndDate?.Date
                .AddHours(formParameters.endHourSelector.Value)
                .AddMinutes(formParameters.endMinuteSelector.Value);
            model.IsFullDate = formParameters.IsFullDate;

            var absence = Absence.Builder()
                .SetUserId(model.UserId)
                .SetAbsenceTypeId(model.AbsenceTypeId ?? 0)
                .SetStatusOfDate(model.StatusOfDates)
                .SetStatusOfType(model.StatusOfType)
                .SetIsFullDate(model.IsFullDate ?? false)
                .SetStartDate(DateOnly.FromDateTime(model.StartDate))
                .SetStartHour(model.StartDate.Hour)
                .SetStartMinute(model.StartDate.Minute)
                .SetEndDate(DateOnly.FromDateTime(model.EndDate.Value))
                .SetEndHour(model.EndDate.Value.Hour)
                .SetEndMinute(model.EndDate.Value.Minute)
                .SetReason(model.Reason)
                .Build();
            
            if (absence.IsFailure)
            {
                return Result.Failure(absence.Error);
            }

            var result = await _absencesService.CreateAsync<Absence>(absence.Value);
            if (result.IsFailure)
            {
                _toastNotificationService.ShowError(result.Error);
                return Result.Failure(result.Error);

            }
            if (result.IsSuccess)
            {
                _toastNotificationService.ShowSuccess(AbsenceSuccessMessages.CreateSuccess.GetDescription());
            }

            return Result.Success();
        }
    }
}
