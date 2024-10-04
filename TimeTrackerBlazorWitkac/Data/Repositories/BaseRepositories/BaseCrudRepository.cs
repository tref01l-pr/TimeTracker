using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using TimeTrackerBlazorWitkac.Interfaces;
using TimeTrackerBlazorWitkac.Interfaces.Repository;

namespace TimeTrackerBlazorWitkac.Data.Repositories.BaseRepositories;

public abstract class BaseCrudRepository<TContext, TEntity, TModel, TKey>(TContext persistentContext, IDbContextFactory<TimeTrackerDbContext> contextFactory, IMapper mapper)
    : BaseFilterableRepository<TContext, TEntity, TKey>(persistentContext, contextFactory, mapper), ICrudRepository<TEntity, TModel, TKey>
    where TContext : DbContext
    where TEntity : class, IDbKey<TKey>
    where TModel : class, IModelKey<TKey>
{
    public async Task<TProjectTo?> GetByIdAsync<TProjectTo>(TKey id) =>
        await _transientContext.Set<TEntity>()
            .Where(e => e.Id!.Equals(id))
            .ProjectTo<TProjectTo>(_mapperConfig)
            .FirstOrDefaultAsync();

    public virtual async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(TModel model)
    {
        try
        {
            var entity = await _persistentContext.Set<TEntity>().AddAsync(_mapper.Map<TModel, TEntity>(model));
            var result = await SaveAsync(_persistentContext);

            if (!result.Value)
            {
                return Result.Failure<TProjectTo>($"Something went wrong during create {typeof(TEntity)}");
            }

            var response = await _persistentContext.Set<TEntity>()
                .Where(e => e.Id!.Equals(entity.Entity.Id))
                .ProjectTo<TProjectTo>(_mapperConfig)
                .FirstOrDefaultAsync();
            
            return response ?? Result.Failure<TProjectTo>($"{typeof(TEntity)} with that id not found!");
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    public virtual async Task<Result<TProjectTo>> UpdateAsync<TProjectTo>(TModel model)
    {
        try
        {
            var entity = await _persistentContext.Set<TEntity>().FindAsync(model.Id);
            if (entity == null)
            {
                return Result.Failure<TProjectTo>($"{typeof(TEntity)} not found");
            }

            _persistentContext.Entry(entity).CurrentValues.SetValues(model);

            var result = await SaveAsync(_persistentContext);

            if (!result.Value)
            {
                return Result.Failure<TProjectTo>($"Something went wrong during create {typeof(TEntity)}");
            }

            var response = await _persistentContext.Set<TEntity>()
                .Where(e => e.Id!.Equals(model.Id))
                .ProjectTo<TProjectTo>(_mapperConfig)
                .FirstOrDefaultAsync();
            
            return response ?? Result.Failure<TProjectTo>($"{typeof(TEntity)} with that id not found!");
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    public virtual async Task<Result> DeleteByIdAsync(TKey id) =>
        await DeleteAsync(x => x.Id!.Equals(id));

    protected virtual async Task<Result> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var entities = await _persistentContext.Set<TEntity>()
                .Where(predicate)
                .ToArrayAsync();
            if (!entities.Any())
            {
                return Result.Failure($"{typeof(TEntity)} not found");
            }

            _persistentContext.Set<TEntity>().RemoveRange(entities);
            var result = await SaveAsync(_persistentContext);
            
            return result.Value
                ? Result.Success()
                : Result.Failure($"Something went wrong during deletion of {typeof(TEntity)}!");
        }
        catch (Exception e)
        {
            return Result.Failure(e.Message);
        }
    }

    protected async Task<Result<bool>> SaveAsync(TContext context) => await context.SaveChangesAsync() > 0;
}