using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Service.Application.BaseServices;

namespace TimeTrackerBlazorWitkac.Service.Application;

public class AbsenceTypesService : CrudService<IAbsenceTypesRepository, AbsenceTypeEntity, AbsenceType, int>, IAbsenceTypesService
{
    public AbsenceTypesService(
        IAbsenceTypesRepository repository, 
        ITransactionRepository transactionRepository) : base(repository, transactionRepository) { }


    public async Task<Result<IList<TProjectTo>>> GetAllAsync<TProjectTo>()
    {
        try
        {
            var result = await _repository.GetAllAsync<TProjectTo>();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<IList<TProjectTo>>(e.Message);
        }
    }
}