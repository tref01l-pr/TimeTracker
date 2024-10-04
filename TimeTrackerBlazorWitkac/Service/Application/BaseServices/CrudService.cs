using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;

namespace TimeTrackerBlazorWitkac.Service.Application.BaseServices;

public abstract class CrudService<TRepository, TEntity, TModel, TKey> : ICrudService<TEntity, TModel, TKey> 
    where TRepository : ICrudRepository<TEntity, TModel, TKey>
{
    protected readonly ITransactionRepository _transactionRepository;
    protected readonly TRepository _repository;
    
    public CrudService(TRepository repository,ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
        _repository = repository;
    }
    
    
    public async Task<Result<TProjectTo?>> GetByIdAsync<TProjectTo>(TKey id)
    {
        try
        {
            return await _repository.GetByIdAsync<TProjectTo>(id);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo?>(e.Message);
        }
    }

    public virtual async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(TModel model)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var result = await _repository.CreateAsync<TProjectTo>(model);
            if (result.IsFailure)
            {
                throw new Exception(result.Error);
            }

            await _transactionRepository.CommitTransactionAsync(transaction);
            return result.Value;
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackTransactionAsync(transaction);
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    public virtual async Task<Result<TProjectTo>> UpdateAsync<TProjectTo>(TModel model)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var result = await _repository.UpdateAsync<TProjectTo>(model);
            if (result.IsFailure)
            {
                throw new Exception(result.Error);
            }
            await _transactionRepository.CommitTransactionAsync(transaction);
            return result.Value;
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackTransactionAsync(transaction);
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    public virtual async Task<Result> DeleteByIdAsync(TKey id)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var result = await _repository.DeleteByIdAsync(id);
            if (result.IsFailure)
            {
                throw new Exception(result.Error);
            }

            await _transactionRepository.CommitTransactionAsync(transaction);
            return Result.Success();
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackTransactionAsync(transaction);
            return Result.Failure(e.Message);
        }
    }
}