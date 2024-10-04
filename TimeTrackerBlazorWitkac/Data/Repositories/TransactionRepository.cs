using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TimeTrackerBlazorWitkac.Interfaces.Repository;

namespace TimeTrackerBlazorWitkac.Data.Repositories;

/// <summary>
/// Repository class for managing database transactions.
/// Implements transaction management functionalities using Entity Framework Core.
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly TimeTrackerDbContext _context;
    private IDbContextTransaction? _transaction = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionRepository"/> class with the specified database context.
    /// </summary>
    /// <param name="context">The database context used for transaction management.</param>
    public TransactionRepository(TimeTrackerDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the transaction object.</returns>
    public async Task<IDbContextTransaction> BeginTransactionAsync(bool keepTransaction = true)
    {
        if (keepTransaction && _transaction != null)
        {
            return _transaction;
        }

        return _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task<Result> RollbackTransactionAsync(IDbContextTransaction transaction)
    {
        if (_transaction == null)
        {
            return Result.Failure("Transaction not opened");
        }

        if (transaction != _transaction)
        {
            return Result.Failure("Transaction not equal to private _transaction");
        }
        
        await _transaction.RollbackAsync();
        _transaction = null;
        return Result.Success();
    }

    public async Task<Result> CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (_transaction == null)
        {
            return Result.Failure("Transaction not opened");
        }

        if (transaction != _transaction)
        {
            return Result.Failure("Transaction not equal to private _transaction");
        }

        await _transaction.CommitAsync();
        _transaction = null;
        return Result.Success();
    }
}