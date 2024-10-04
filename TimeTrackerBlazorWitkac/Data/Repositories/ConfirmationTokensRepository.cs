using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Repositories;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Data.Repositories.BaseRepositories;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;

namespace TimeTrackerBlazorWitkac.Data.Repositories;

public class ConfirmationTokensRepository(TimeTrackerDbContext context, IDbContextFactory<TimeTrackerDbContext> contextFactory, IMapper mapper) 
    : BaseCrudRepository<TimeTrackerDbContext, ConfirmationTokenEntity, ConfirmationToken, int>(context, contextFactory, mapper), IConfirmationTokensRepository
{
    public async Task<TProjectTo?> GetByTokenAsync<TProjectTo>(string token) =>
        await _transientContext.ConfirmationTokens
            .AsNoTracking()
            .Where(ct => ct.Token == token)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .FirstOrDefaultAsync();

    public override async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(ConfirmationToken model)
    {
        var confirmTokensExist = await _persistentContext.ConfirmationTokens
            .Where(ct => 
                ct.UserId == model.UserId && 
                ct.ConfirmationType == model.ConfirmationType)
            .ProjectTo<ConfirmationTokenResponse>(_mapperConfig)
            .ToListAsync();

        if (confirmTokensExist.Count > 1)
        {
            var clearConfirmationTokensResult = await DeleteByUserIdWithType(model.UserId, model.ConfirmationType);
            if (clearConfirmationTokensResult.IsFailure)
            {
                return Result.Failure<TProjectTo>(clearConfirmationTokensResult.Error);
            }
        }
        else if (confirmTokensExist.Count == 1)
        {
            return await base.UpdateAsync<TProjectTo>(model with { Id = confirmTokensExist.First().Id });
        }
        
        return await base.CreateAsync<TProjectTo>(model);
    }

    public override Task<Result<TProjectTo>> UpdateAsync<TProjectTo>(ConfirmationToken model) =>
        Task.FromResult(Result.Failure<TProjectTo>(ConfirmationTokensRepositoryErrorMessages.UpdateMethodNotAllowed.GetDescription()));

    public async Task<Result> DeleteByUserIdWithType(string userExistId, ConfirmationTypes confirmationType) =>
        await DeleteAsync(ct => ct.UserId == userExistId && ct.ConfirmationType == confirmationType);
}