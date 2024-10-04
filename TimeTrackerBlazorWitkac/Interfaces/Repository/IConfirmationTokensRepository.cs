using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

public interface IConfirmationTokensRepository : ICrudRepository<ConfirmationTokenEntity, ConfirmationToken, int>
{
    Task<TProjectTo?> GetByTokenAsync<TProjectTo>(string token);
    Task<Result> DeleteByUserIdWithType(string userExistId, ConfirmationTypes email);
}