using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Data.Repositories.BaseRepositories;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.QueryTags;

namespace TimeTrackerBlazorWitkac.Data.Repositories;

/// <summary>
/// Repository class for managing Company entities.
/// This class provides methods to perform CRUD operations and retrieve company records from the database.
/// </summary>
public class CompaniesRepository : BaseCrudRepository<TimeTrackerDbContext, CompanyEntity, Company, int>, ICompaniesRepository
{
    /// <summary>
    /// Initializes a new instance of the repository with the specified context and mapper.
    /// </summary>
    /// <param name="context">The database context used for data access.</param>
    /// <param name="contextFactory">The factory for creating database context instances.</param>
    /// <param name="mapper">The object mapper for converting entity types to model types.</param>
    public CompaniesRepository(
        TimeTrackerDbContext context,
        IDbContextFactory<TimeTrackerDbContext> contextFactory,
        IMapper mapper)
        : base(context, contextFactory, mapper) { }

    /// <summary>
    /// Retrieves all Company records from the database.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the Company records to.</typeparam>
    /// <returns>A list of Company records projected to the specified type.</returns>
    public async Task<IList<TProjectTo>> GetAllAsync<TProjectTo>() =>
        await _transientContext.Companies
            .TagWith(CompanyQueryTags.GetAllCompanies.GetDescription())
            .AsNoTracking()
            .ProjectTo<TProjectTo>(_mapperConfig)
            .ToListAsync();

    /// <summary>
    /// Retrieves a Company record by its name.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the Company record to.</typeparam>
    /// <param name="name">The name of the Company to retrieve.</param>
    /// <returns>A Company record projected to the specified type, or null if not found.</returns>
    public async Task<TProjectTo?> GetByNameAsync<TProjectTo>(string name) =>
        await _transientContext.Companies
            .TagWith(CompanyQueryTags.GetCompanyByName.GetDescription())
            .Where(c => c.Name == name)
            .AsNoTracking()
            .ProjectTo<TProjectTo>(_mapperConfig)
            .FirstOrDefaultAsync();
}