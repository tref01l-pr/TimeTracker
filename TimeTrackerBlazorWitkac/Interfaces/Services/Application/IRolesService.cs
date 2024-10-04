using CSharpFunctionalExtensions;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;

public interface IRolesService
{
    Task<Result<IList<string>>> GetAllAsync();
    Task<Result<string?>> GetByNameAsync(string name);
}