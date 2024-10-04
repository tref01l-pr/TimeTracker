using AutoMapper;
using BlazorBootstrap;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Models;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Details.Calendar;
using TimeTrackerBlazorWitkac.Components.Account.Pages.Manage.Components.Models;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Resources;
using TimeTrackerBlazorWitkac.Resources.Calendar;
using TimeTrackerBlazorWitkac.Resources.User;

namespace TimeTrackerBlazorWitkac.Service
{
    public class SchedulerService : ISchedulerService
    {
        private readonly TooltipService _tooltipService;
        private readonly ToastService _toastService;
        private readonly DialogService _dialogService;
        private readonly PreloadService _preloadService;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;
        private readonly IAttendancesService _attendancesService;
        private readonly IAbsencesService _absencesService;
        private readonly IAttendancesRepository _attendancesRepository;
        private readonly IToastNotificationService _notificationService;
        private readonly IHolidaysService _holidaysService;
        private readonly IJSRuntime _jsRunTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerService"/> class.
        /// </summary>
        public SchedulerService(
            TooltipService tooltipService,
            DialogService dialogService,
            UserManager<UserEntity> userManager,
            IMapper mapper,
            IAttendancesService attendancesService,
            IAbsencesService absencesService,
            IToastNotificationService notificationService,
            ToastService toastService,
            IHolidaysService holidaysService,
            IJSRuntime jsRunTime,
            PreloadService preloadService)
        {
            _tooltipService = tooltipService;
            _dialogService = dialogService;
            _userManager = userManager;
            _mapper = mapper;
            _attendancesService = attendancesService;
            _absencesService = absencesService;
            _notificationService = notificationService;
            _toastService = toastService;
            _holidaysService = holidaysService;
            _jsRunTime = jsRunTime;
            _preloadService = preloadService;
        }
        /// <summary>
        /// Creates and returns tooltip options with default settings.
        /// </summary>
        /// <returns>Returns default <see cref="TooltipOptions"/>.</returns>
        public TooltipOptions OptionTooltip()
        {
            return new TooltipOptions
            {
                Position = TooltipPosition.Bottom,
                Style = $"background: {Constants.DefaultBackgroundToolTipColor}; color: {Constants.DefaultToolTipColor}",
                Duration = null
            };
        }
        /// <summary>
        /// Creates a <see cref="ToastMessage"/> object for displaying notifications.
        /// </summary>
        /// <param name="toastType">The type of the toast (info, success, warning, etc.).</param>
        /// <param name="title">The title of the toast message.</param>
        /// <param name="dateTime">The date/time information to display in the toast.</param>
        /// <param name="message">The main message body of the toast.</param>
        /// <returns>Returns a configured <see cref="ToastMessage"/> object.</returns>
        private ToastMessage CreateToastMessage(ToastType toastType, string title, string dateTime, string message)
        {
            return new ToastMessage
            {
                Type = toastType,
                Title = title,
                HelpText = dateTime,
                Message = message
            };
        }
        /// <summary>
        /// Handles the mouse leave event for an appointment and closes the tooltip.
        /// </summary>
        /// <param name="args">The event arguments for the appointment mouse leave event.</param>
        public void OnMouseLeaveEvent(SchedulerAppointmentMouseEventArgs<WorkDayResponse> args)
        {
            _tooltipService.Close();
        }
        /// <summary>
        /// Handles the event when a time slot is selected in the scheduler.
        /// </summary>
        /// <param name="args">The arguments of the slot select event.</param>
        /// <param name="user">The current user selecting the slot.</param>
        /// <param name="workDays">The list of work days to update.</param>
        /// <param name="scheduler">The scheduler instance.</param>
        /// <param name="toastMessages">A list of toast messages for notifications.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>


        public async Task OnSlotSelectEvent(SchedulerSlotSelectEventArgs args, UserEntity user,
            IList<WorkDayResponse> workDays, RadzenScheduler<WorkDayResponse> scheduler,
            List<ToastMessage> toastMessages)
        {
            if (user == null) return;

            try
            {
                await _jsRunTime.InvokeVoidAsync("localStorage.setItem", "selectedCalendarDate", args.Start.ToString("yyyy-MM-dd"));
                var parameters = new AbsenceFormParameters
                {
                    StartTime = args.Start,
                    EndTime = args.End,
                    StartHour = args.Start.Hour,
                    StartMinute = args.Start.Minute,
                    EndHour = args.End.Hour,
                    EndMinute = args.End.Minute,
                    User = _mapper.Map<UserEntity, UserResponse>(user),
                };

                var data = await _dialogService.OpenAsync<ModalAbsences>(SchedulerTitle.AbsenceTitle.GetDescription(), new Dictionary<string, object> { { SchedulerParameters.Parameters.GetDescription(), parameters } });

                if (data != null)
                {
                    workDays.Add(data);
                    var savedDate = await _jsRunTime.InvokeAsync<string>("localStorage.getItem", "selectedCalendarDate");
                    scheduler.CurrentDate = DateTime.Parse(savedDate);
                }
                await scheduler.Reload();
            }
            catch (Exception ex)
            {
                _notificationService.ShowError(ex.Message);
            }
        }
        /// <summary>
        /// Handles the rendering of each slot in the scheduler.
        /// </summary>
        /// <param name="args">The event arguments for the slot render event.</param>
        /// <param name="toastMessages">A list of toast messages for notifications.</param>
        /// <param name="workDays">The list of work days in the calendar.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task OnSlotRender(SchedulerSlotRenderEventArgs args, List<ToastMessage> toastMessages,
            IList<WorkDayResponse> workDays)
        {
                // Disable past dates
                if (args.Start.Date < DateTime.Now)
                {
                    args.Attributes["style"] =
                        $"background: {Constants.DefaultBackgroundDisablePastDates}; pointer-events: {Constants.DefaultPointerEvent}; cursor: {Constants.DefaultCursor};";
                }

                // Highlight today's date in the month view
                if (args.View.Text == "Month" && args.Start.Date == DateTime.Today)
                {
                    args.Attributes["style"] =
                        $"background: {Constants.DefaultBackgroundHighlightingDates};";
                }

                // Highlight specific hours in week or day views
                if ((args.View.Text == "Week" || args.View.Text == "Day") && args.Start.Hour > 8 && args.Start.Hour < 24)
                {
                    args.Attributes["style"] =
                       $"background: {Constants.DefaultBackgroundHighlightingDates};";
                }

                var slotWorkDays = workDays?.FirstOrDefault(h => h.StartDate.Date == args.Start.Date);
                if (slotWorkDays != null && slotWorkDays.WorkDayType == WorkDayType.Holiday)
                {
                    args.Attributes["style"] = $"background: {Constants.DefaultBackgroundHighlightingDates}; color: {Constants.DefaultColor};";
                    args.Attributes["class"] = "Holiday";
                    args.Attributes["after-text"] = $"{slotWorkDays.Name}";
                }
            
        }
        /// <summary>
        /// Dynamically sets the height of a calendar slot in the scheduler based on the number of work days.
        /// </summary>
        /// <param name="slotWorkDays">The number of work days in the current slot.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SetSlotHeight(int slotWorkDays)
        {
            var height = slotWorkDays * 50;
            await _jsRunTime.InvokeVoidAsync("setElementHeight", height);
        }
        /// <summary>
        /// Handles the rendering of each appointment in the scheduler.
        /// </summary>
        /// <param name="args">The event arguments for the appointment render event.</param>
        /// <param name="workDays">The list of work days to check for rendering rules.</param>
        public void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<WorkDayResponse> args, IList<WorkDayResponse> workDays, UserEntity user)
        {
            var result = workDays.Where(h =>
                   h.StartDate.Date <= args.Data.StartDate.Date &&
                   h.EndDate.Value.Date >= args.Data.EndDate.Value.Date).ToList();
            switch (args.Data.WorkDayType)
            {
                case WorkDayType.Holiday:
                    RenderingHolidayType(args);
                    break;
                case WorkDayType.Absence:
                    RenderingAbsenceType(args, user);
                    break;
                case WorkDayType.Attendance:
                    RenderingAttendanceType(args);
                    break;

            }
        }

        /// <summary>
        /// Handles the selection of an appointment in the scheduler.
        /// </summary>
        /// <param name="args">The arguments of the appointment select event.</param>
        /// <param name="user">The current user.</param>
        /// <param name="scheduler">The scheduler instance.</param>
        /// <param name="toastMessages">A list of toast messages for notifications.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>


        public async Task HandleAppointmentSelectAsync(SchedulerAppointmentSelectEventArgs<WorkDayResponse> args,
            UserEntity user, RadzenScheduler<WorkDayResponse> scheduler, List<ToastMessage> toastMessages)
        {
            if (user == null) return;
            var copy = _mapper.Map<WorkDayResponse>(args.Data);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            try
            {
                if (IsAdmin(userRole) && args.Data.UserId != user.Id && args.Data.WorkDayType == WorkDayType.Absence)
                {
                    var updatedData = await OnSlotSelectDetail(args.Data);
                    if (updatedData != null) UpdateAbsenceData(args.Data, updatedData);
                }
                else
                {
                    await OnSlotSelectRenderEdit(args, user, scheduler, copy);
                }
                await scheduler.Reload();
            }
            catch(Exception e)
            {
                _notificationService.ShowError(e.Message);
            }
        }
        private bool IsAdmin(string userRole) => userRole == UserRoles.Admin.GetDescription();
        /// <summary>
        /// Retrieves initial data for the scheduler based on a date range.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <param name="user">The current user making the request.</param>
        /// <returns>A result containing a list of <see cref="WorkDayResponse"/> objects.</returns>

        public async Task<Result<IList<WorkDayResponse>>> GetInitialDataByDate(DateOnly startDate, DateOnly endDate, UserEntity user)
        {
            try
            {
          
                var absencesResult =
                    await _absencesService.GetFilteredAsync<WorkDayResponse>(startDate: startDate, endDate: endDate);
                if (absencesResult.IsFailure) return Result.Failure<IList<WorkDayResponse>>(absencesResult.Error);

                var holidaysResult = await _holidaysService.GetAllAsync<WorkDayResponse>();
                if (holidaysResult.IsFailure) return Result.Failure<IList<WorkDayResponse>>(holidaysResult.Error);

                var response = absencesResult.Value.Items.Concat(holidaysResult.Value).ToList();
              
                return Result.Success(response).Value;
               
            }
            finally
            {
            
            }

        }

        /// <summary>
        /// Retrieves initial data for a specific user within a date range.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="user">The current user making the request.</param>
        /// <returns>A result containing a list of <see cref="WorkDayResponse"/> objects.</returns>
        public async Task<Result<IList<WorkDayResponse>>> GetInitialDataByDateUser(DateOnly startDate, DateOnly endDate,
            string userId, UserEntity user)

        {
            var absencesResult = await _absencesService
                .GetFilteredAsync<WorkDayResponse>(startDate: startDate, endDate: endDate, userId: userId)
                .ConfigureAwait(false);
            if (absencesResult.IsFailure) return Result.Failure<IList<WorkDayResponse>>(absencesResult.Error);

            var attendancesResult =
                       await _attendancesService.GetFilteredAsync<WorkDayResponse>(startDate: startDate, endDate: endDate,
                           userId: userId);
            if (attendancesResult.IsFailure) return Result.Failure<IList<WorkDayResponse>>(attendancesResult.Error);

            var holidaysResult = await _holidaysService.GetAllAsync<WorkDayResponse>();

            var response = absencesResult.Value.Items
                            .Concat(attendancesResult.Value.Items)
                            .Concat(holidaysResult.Value)
                            .ToList();

            return Result.Success(response).Value;
        }
        /// <summary>
        /// Opens a dialog for editing an absence event.
        /// </summary>
        /// <param name="copy">The <see cref="WorkDayResponse"/> object to edit.</param>
        /// <param name="user">The current user making the request.</param>
        /// <param name="scheduler">The scheduler instance.</param>
        /// <returns>Returns the updated <see cref="WorkDayResponse"/> object.</returns
        private async Task<WorkDayResponse> OnSlotSelectEdit(WorkDayResponse copy, UserEntity user, RadzenScheduler<WorkDayResponse> scheduler)
        {
            try
            {
                var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                return await _dialogService.OpenAsync<EditAbsences>(SchedulerTitle.DetailTitle.GetDescription(), new Dictionary<string, object> { { SchedulerParameters.WorkDayParameter.GetDescription(), copy }, { SchedulerParameters.RoleParameter.GetDescription(), userRole }, { SchedulerParameters.SchedulerParameter.GetDescription(), scheduler } });
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Opens a dialog for editing an absence event.
        /// </summary>
        /// <param name="copy">The <see cref="WorkDayResponse"/> object to edit.</param>
        /// <param name="user">The current user making the request.</param>
        /// <param name="scheduler">The scheduler instance.</param>
        /// <returns>Returns the updated <see cref="WorkDayResponse"/> object.</returns>
        private async Task OnSlotSelectRenderEdit(SchedulerAppointmentSelectEventArgs<WorkDayResponse> args, UserEntity user, RadzenScheduler<WorkDayResponse> scheduler, WorkDayResponse copy)
        {
            if (args.Data.UserId == user.Id && args.Data.WorkDayType == WorkDayType.Absence)
            {
                var data = await OnSlotSelectEdit(copy, user, scheduler);
                if (data != null)
                {
                    UpdateAbsenceData(args.Data, data);
                    await scheduler.Reload();

                }
            }
        }



        /// <summary>
        /// Opens a dialog for viewing the details of an absence event.
        /// </summary>
        /// <param name="copy">The <see cref="WorkDayResponse"/> object to view.</param>
        /// <returns>Returns the detailed <see cref="WorkDayResponse"/> object.</returns>
        private async Task<WorkDayResponse> OnSlotSelectDetail(WorkDayResponse copy)
        {
            try
            {
                var obj = new DetailAbsencesModel { Absence = copy };

                return await _dialogService.OpenAsync<DetailAbsences>(SchedulerTitle.DetailTitle.GetDescription(), new Dictionary<string, object> { { SchedulerParameters.ModelParameter.GetDescription(), obj } });
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Updates an existing <see cref="WorkDayResponse"/> object with new data.
        /// </summary>
        /// <param name="target">The target object to update.</param>
        /// <param name="source">The source object containing updated data.</param>
        private void UpdateAbsenceData(WorkDayResponse target, WorkDayResponse source)
        {
            _mapper.Map(source, target);
        }
        #region Helper Methods 
        private void RenderingHolidayType(SchedulerAppointmentRenderEventArgs<WorkDayResponse> args)
        {
            args.Attributes["style"] = "display: none";
        }
        private void RenderingAbsenceType(SchedulerAppointmentRenderEventArgs<WorkDayResponse> args, UserEntity user)
        {
            if (args.Data.IsFullyConfirmed == ConfirmationStatus.Pending)
            {
                SetAbsenceStyle(args, args.Data.AbsenceType?.PendingBackgroundColor, args.Data.AbsenceType?.PendingIconColor,
                    args.Data.UserId == user.Id ? Constants.DefaultOpacityByUser : Constants.DefaultOpacity);
            }
            else if (args.Data.IsFullyConfirmed == ConfirmationStatus.Confirmed)
            {
                SetAbsenceStyle(args, args.Data.AbsenceType?.AcceptedBackgroundColor, args.Data.AbsenceType?.AcceptedIconColor,
                    args.Data.UserId == user.Id ? Constants.DefaultOpacityByUser : Constants.DefaultOpacity);
            }
        }
        private void SetAbsenceStyle(SchedulerAppointmentRenderEventArgs<WorkDayResponse> args, string backgroundColor, string iconColor, double opacity)
        {
            args.Attributes["style"] = $"background: {backgroundColor}; color: {iconColor}; filter: opacity({opacity}%);";
        }
        private void RenderingAttendanceType(SchedulerAppointmentRenderEventArgs<WorkDayResponse> args)
        {
            args.Attributes["style"] = $"background-color: {Constants.DefaultAttendanceBackgroundColor} !important; color: {Constants.DefaultColor};";
        }
        #endregion
    }
}