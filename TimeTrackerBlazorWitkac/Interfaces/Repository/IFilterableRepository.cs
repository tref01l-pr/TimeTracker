using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Options;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

public interface IFilterableRepository<TEntity, TKey>
{
    Task<Result<PageResponse<TProjectTo>>> GetFilteredPageAsync<TProjectTo>(
        PaginationOptions? options = null,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>>? filterBy = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? orderBy = null,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default);
}