using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Data.Repositories.BaseRepositories;
using TimeTrackerBlazorWitkac.Interfaces.Repository;

namespace TimeTrackerBlazorWitkac.Data.Repositories;

/// <summary>
/// Repository class for managing AbsenceType entities.
/// </summary>
public class AbsenceTypesRepository : BaseCrudRepository<TimeTrackerDbContext, AbsenceTypeEntity, AbsenceType, int>, IAbsenceTypesRepository
{
    /// <summary>
    /// Initializes a new instance of the repository with the specified context and mapper.
    /// </summary>
    /// <param name="contextFactory"></param>
    /// <param name="mapper">The object mapper for entity-to-model conversion.</param>
    /// <param name="context"></param>
    public AbsenceTypesRepository(
        TimeTrackerDbContext context,
        IDbContextFactory<TimeTrackerDbContext> contextFactory,
        IMapper mapper)
        : base(context, contextFactory, mapper) { }
    
    /// <summary>
    /// Retrieves all AbsenceType records from the database.
    /// </summary>
    /// <returns>An list of AbsenceType records.</returns>
    public async Task<IList<TProjectTo>> GetAllAsync<TProjectTo>() =>
        await _transientContext.AbsenceTypes
            .TagWith("Get all AbsenceTypes")
            .AsNoTracking()
            .ProjectTo<TProjectTo>(_mapperConfig)
            .ToListAsync();
}