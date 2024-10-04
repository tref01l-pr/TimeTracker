using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Storage;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

/// <summary>
/// Defines the contract for transaction management within the repository.
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, with a transaction as the result.</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(bool keepTransaction = true);

    Task<Result> CommitTransactionAsync(IDbContextTransaction transaction);
    Task<Result> RollbackTransactionAsync(IDbContextTransaction transaction);
}