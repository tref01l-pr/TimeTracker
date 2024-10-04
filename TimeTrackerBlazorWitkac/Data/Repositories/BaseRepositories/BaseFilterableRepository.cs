using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Options;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace TimeTrackerBlazorWitkac.Data.Repositories.BaseRepositories;

public abstract class BaseFilterableRepository<TContext, TEntity, TKey> : IFilterableRepository<TEntity, TKey>  
    where TContext : DbContext
    where TEntity : class, IDbKey<TKey> 
{
    private readonly IDbContextFactory<TimeTrackerDbContext> _contextFactory;
    protected readonly IMapper _mapper;
    protected readonly TContext _persistentContext;
    

    public BaseFilterableRepository(TContext persistentContext, IDbContextFactory<TimeTrackerDbContext> contextFactory, IMapper mapper)
    {
        _persistentContext = persistentContext;
        _contextFactory = contextFactory;
        _mapper = mapper;
    }
    
    protected IConfigurationProvider _mapperConfig => _mapper.ConfigurationProvider;
    protected TimeTrackerDbContext _transientContext => _contextFactory.CreateDbContext();
    
    public virtual async Task<Result<PageResponse<TProjectTo>>> GetFilteredPageAsync<TProjectTo>(
        PaginationOptions? options = null,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>>? filterBy = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? orderBy = null,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _transientContext.Set<TEntity>().AsQueryable();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (filterBy != null)
            {
                query = await filterBy(query);
            }
        
            if (parameters != null)
            {
                
            }
        
            query = orderBy != null 
                ? orderBy(query) 
                : query.OrderBy(entity => entity.Id);
            
            var totalItems = await query.CountAsync(cancellationToken);
            if (options != null)
            {
                query = query.Skip((options.Page - 1) * options.PageSize)
                    .Take(options.PageSize);
            }
        
            var items = await query
                
                .ProjectTo<TProjectTo>(_mapperConfig)
                .ToListAsync(cancellationToken);

            return new PageResponse<TProjectTo>(items, totalItems);
        }
        catch (Exception e)
        {
            return Result.Failure<PageResponse<TProjectTo>>(e.Message);
        }
    }
}