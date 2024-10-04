using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Options;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;

public interface IUsersService : ICrudService<UserEntity, User, string>
{
    Task<Result<TProjectTo?>> GetByEmailAsync<TProjectTo>(string email);
    Task<Result<PageResponse<TProjectTo>>> GetFilteredPageAsync<TProjectTo>(PaginationOptions? options = null,
        string? email = null, CancellationToken cancellationToken = default);
}