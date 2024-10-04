using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;

public interface IAbsenceTypesService : ICrudService<AbsenceTypeEntity, AbsenceType, int>
{
    Task<Result<IList<TProjectTo>>> GetAllAsync<TProjectTo>();
}