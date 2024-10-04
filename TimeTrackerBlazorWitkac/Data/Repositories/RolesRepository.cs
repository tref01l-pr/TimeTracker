using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Radzen;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Repositories;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Data.Repositories.BaseRepositories;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;

namespace TimeTrackerBlazorWitkac.Data.Repositories;

public class RolesRepository(TimeTrackerDbContext context, IDbContextFactory<TimeTrackerDbContext> contextFactory, IMapper mapper) 
    : BaseCrudRepository<TimeTrackerDbContext, RoleEntity, Role, string>(context, contextFactory, mapper), IRolesRepository
{
    public async Task<IList<TProjectTo>> GetAllAsync<TProjectTo>() =>
        await _transientContext.Roles
            .AsNoTracking()
            .ProjectTo<TProjectTo>(_mapperConfig)
            .ToListAsync();

    public async Task<TProjectTo?> GetByNameAsync<TProjectTo>(string name) =>
        await _transientContext.Roles
            .AsNoTracking()
            .Where(r => r.Name == name)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .FirstOrDefaultAsync();

    public override async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(Role model)
    {
        try
        {
            var roleExist = await _persistentContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Name == model.Name);
            if (roleExist != null)
            {
                return Result.Failure<TProjectTo>(RolesRepositoryErrorMessages.RoleAlreadyExists.GetDescription());
            }
            var roleEntity = new RoleEntity();
            roleEntity.Name = model.Name;
            await _persistentContext.Roles.AddAsync(roleEntity);
            var result = await SaveAsync(_persistentContext);

            if (!result.Value)
            {
                return Result.Failure<TProjectTo>(RolesRepositoryErrorMessages.CreationFailed.GetDescription());
            }
        
            var response = await _persistentContext.Roles
                .Where(e => e.Id.Equals(roleEntity.Id))
                .ProjectTo<TProjectTo>(_mapperConfig)
                .FirstOrDefaultAsync();
            
            return response ?? Result.Failure<TProjectTo>(RolesRepositoryErrorMessages.RoleNotFound.GetDescription());
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }
}