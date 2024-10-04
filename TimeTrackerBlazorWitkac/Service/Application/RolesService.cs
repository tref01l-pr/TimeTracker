using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Service.Application.BaseServices;

namespace TimeTrackerBlazorWitkac.Service.Application;

public class RolesService : CrudService<IRolesRepository, RoleEntity, Role, string>, IRolesService
{
    public RolesService(IRolesRepository repository, ITransactionRepository transactionRepository) : base(repository, transactionRepository) { }
    
    public async Task<Result<IList<string>>> GetAllAsync()
    {
        try
        {
            var result = await _repository.GetAllAsync<string>();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<IList<string>>(e.Message);
        }
    }

    public async Task<Result<string?>> GetByNameAsync(string name)
    {
        try
        {
            var result = await _repository.GetByNameAsync<string>(name);
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<string?>(e.Message);
        }
    }
}