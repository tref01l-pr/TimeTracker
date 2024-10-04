using CSharpFunctionalExtensions;

namespace TimeTrackerBlazorWitkac.Interfaces.Services;


public interface IBaseAbsenceStatusService
{
    Task<Result<TProjectTo>> ConfirmTypeAbsence<TProjectTo>(int id);
    Task<Result<TProjectTo>> ConfirmDatesAbsence<TProjectTo>(int id);
    Task<Result> DeleteDate(WorkDayResponse workDayResponse);
}
public interface IAbsenceStatusService : IBaseAbsenceStatusService
{
    void OnClose(WorkDayResponse model);

}