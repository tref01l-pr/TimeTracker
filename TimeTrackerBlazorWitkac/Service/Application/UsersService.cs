using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Options;
using TimeTrackerBlazorWitkac.Service.Application.BaseServices;

namespace TimeTrackerBlazorWitkac.Service.Application;

public class UsersService : CrudService<IUsersRepository, UserEntity, User, string>, IUsersService
{
    public UsersService(IUsersRepository repository, ITransactionRepository transactionRepository) 
        : base(repository, transactionRepository) { }

    public async Task<Result<TProjectTo?>> GetByEmailAsync<TProjectTo>(string email)
    {
        try
        {
            var result = await _repository.GetByEmailAsync<TProjectTo>(email);
            return result;
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo?>(e.Message);
        }
    }

    public async Task<Result<PageResponse<TProjectTo>>> GetFilteredPageAsync<TProjectTo>(PaginationOptions? options = null, string? email = null, CancellationToken cancellationToken = default)
    {
        IList<Expression<Func<UserEntity, bool>>> predicates = new List<Expression<Func<UserEntity, bool>>>();

        if (!string.IsNullOrEmpty(email))
        {
            predicates.Add(u => u.Email!.Contains(email));
        }

        return await _repository.GetFilteredPageAsync<TProjectTo>(
            options,
            PredicateCombiner.Combine<UserEntity>(predicates.ToArray()),
            cancellationToken: cancellationToken);
    }

    public override Task<Result<TProjectTo>> CreateAsync<TProjectTo>(User model)
    {
        return Task.FromResult(Result.Failure<TProjectTo>("Use registration to create user"));
    }
}