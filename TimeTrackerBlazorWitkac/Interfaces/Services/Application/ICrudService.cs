using CSharpFunctionalExtensions;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;

public interface ICrudService<TEntity, TModel, TKey>
{
    Task<Result<TProjectTo?>> GetByIdAsync<TProjectTo>(TKey id);
    Task<Result<TProjectTo>> CreateAsync<TProjectTo>(TModel model);
    Task<Result<TProjectTo>> UpdateAsync<TProjectTo>(TModel model);

    Task<Result> DeleteByIdAsync(TKey id);
}