using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Storage;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

public interface ICrudRepository<TEntity, TModel, TKey> : IFilterableRepository<TEntity, TKey>
{
    Task<TProjectTo?> GetByIdAsync<TProjectTo>(TKey id);
    Task<Result<TProjectTo>> CreateAsync<TProjectTo>(TModel model);
    Task<Result<TProjectTo>> UpdateAsync<TProjectTo>(TModel model);

    Task<Result> DeleteByIdAsync(TKey id);
}