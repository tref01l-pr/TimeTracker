using BlazorBootstrap;
using CSharpFunctionalExtensions;
using Radzen;
using Radzen.Blazor;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data;
using TimeTrackerBlazorWitkac.Data.Entities;

namespace TimeTrackerBlazorWitkac.Interfaces;

public interface IBaseSchedulerService
{
    Task OnSlotRender(SchedulerSlotRenderEventArgs args, List<ToastMessage> toastMessages, IList<WorkDayResponse> workDays);
    Task OnSlotSelectEvent(SchedulerSlotSelectEventArgs args, UserEntity user, IList<WorkDayResponse> wordDays, RadzenScheduler<WorkDayResponse> scheduler, List<ToastMessage> toastMessages);
    Task HandleAppointmentSelectAsync(SchedulerAppointmentSelectEventArgs<WorkDayResponse> args, UserEntity user, RadzenScheduler<WorkDayResponse> scheduler, List<ToastMessage> toastMessages);
    void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<WorkDayResponse> args, IList<WorkDayResponse> workDays, UserEntity user);
}
public interface ISchedulerService : IBaseSchedulerService
{
    TooltipOptions OptionTooltip();
    void OnMouseLeaveEvent(SchedulerAppointmentMouseEventArgs<WorkDayResponse> args);
    Task<Result<IList<WorkDayResponse>>> GetInitialDataByDate(DateOnly startDate, DateOnly endDate, UserEntity user);
    Task<Result<IList<WorkDayResponse>>> GetInitialDataByDateUser(DateOnly startDate, DateOnly endDate, string userId, UserEntity user);

}

