using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Service.Application.BaseServices;

namespace TimeTrackerBlazorWitkac.Service.Application;

public class HolidaysService : CrudService<IHolidaysRepository, HolidayEntity, Holiday, int>, IHolidaysService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HolidaysService"/> class.
    /// </summary>
    /// <param name="repository">The holidays repository for data operations.</param>
    /// <param name="transactionRepository">The repository for handling transactions.</param>
    public HolidaysService(
        IHolidaysRepository repository, 
        ITransactionRepository transactionRepository) : base(repository, transactionRepository) { }
    
    /// <summary>
    /// Retrieves all holiday records asynchronously and returns them as a result.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <returns>A <see cref="Result"/> containing the list of projected holidays.</returns>
    public async Task<Result<IList<TProjectTo>>> GetAllAsync<TProjectTo>()
    {
        try
        {
            var result = await _repository.GetAllAsync<TProjectTo>();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<IList<TProjectTo>>(e.Message);
        }
    }

    /// <summary>
    /// Retrieves holiday records for a specific year and month asynchronously.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <param name="date">The date specifying the year and month.</param>
    /// <returns>A <see cref="Result"/> containing the list of projected holidays for the specified year and month.</returns>
    public async Task<Result<IList<TProjectTo>>> GetByYearMonthAsync<TProjectTo>(DateOnly date)
    {
        try
        {
            var result = await _repository.GetByYearMonthAsync<TProjectTo>(date);
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<IList<TProjectTo>>(e.Message);
        }
    }

    /// <summary>
    /// Retrieves holiday records within the specified date range asynchronously.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A <see cref="Result"/> containing the list of projected holidays within the specified date range.</returns>
    public async Task<Result<IList<TProjectTo>>> GetByDates<TProjectTo>(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            var result = await _repository.GetByDates<TProjectTo>(startDate, endDate);
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<IList<TProjectTo>>(e.Message);
        }
    }

    /// <summary>
    /// Retrieves a holiday record by its start date and summary asynchronously.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holiday to.</typeparam>
    /// <param name="date">The start date of the holiday.</param>
    /// <param name="summary">The summary of the holiday.</param>
    /// <returns>A <see cref="Result"/> containing the projected holiday record or null if not found.</returns>
    public async Task<Result<TProjectTo?>> GetByDateWithSummaryAsync<TProjectTo>(DateOnly date, string summary)
    {
        try
        {
            var result = await _repository.GetByDateWithSummaryAsync<TProjectTo>(date, summary);
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo?>(e.Message);
        }
    }

    /// <summary>
    /// Creates a new holiday record asynchronously.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the created holiday to.</typeparam>
    /// <param name="model">The holiday model to create.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure along with the projected holiday.</returns>
    public override async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(Holiday model)
    {
        try
        {
            var holidayExist = await _repository.GetByDateWithSummaryAsync<Holiday>(model.StartDate, model.Name);
            if (holidayExist != null)
            {
                throw new Exception(HolidaysServiceErrorMessages.HolidayAlreadyExists.GetDescription());
            }
            
            return await base.CreateAsync<TProjectTo>(model);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    /// <summary>
    /// Creates multiple holiday records asynchronously.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the created holidays to.</typeparam>
    /// <param name="holidays">The holidays to create.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure along with the list of projected holidays.</returns>
    public async Task<Result<IList<TProjectTo>>> CreateAsync<TProjectTo>(params Holiday[] holidays)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            IList<TProjectTo> resultList = new List<TProjectTo>();
            foreach (var holiday in holidays)
            {
                var holidayExist = await _repository.GetByDateWithSummaryAsync<Holiday>(holiday.StartDate, holiday.Name);
                if (holidayExist != null)
                {
                    throw new Exception(HolidaysServiceErrorMessages.HolidayAlreadyExists.GetDescription());
                }
                
                var result = await _repository.CreateAsync<TProjectTo>(holiday);
                if (result.IsFailure)
                {
                    throw new Exception(result.Error);
                }
                
                resultList.Add(result.Value);
            }
            
            await _transactionRepository.CommitTransactionAsync(transaction);
            return Result.Success(resultList);
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackTransactionAsync(transaction);
            return Result.Failure<IList<TProjectTo>>(e.Message);
        }
    }

    /// <summary>
    /// Updates an existing holiday record asynchronously.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the updated holiday to.</typeparam>
    /// <param name="model">The holiday model to update.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure along with the projected holiday.</returns>
    public override async Task<Result<TProjectTo>> UpdateAsync<TProjectTo>(Holiday model)
    {
        try
        {
            var holiday = await _repository.GetByIdAsync<Holiday>(model.Id);

            if (holiday == null)
            {
                throw new Exception(HolidaysServiceErrorMessages.HolidayNotFound.GetDescription());
            }

            if (holiday.StartDate == model.StartDate && holiday.Name == model.Name)
            {
                return await base.UpdateAsync<TProjectTo>(model);
            }

            var holidayExist = await _repository.GetByDateWithSummaryAsync<Holiday>(model.StartDate, model.Name);
            if (holidayExist != null)
            {
                throw new Exception(HolidaysServiceErrorMessages.HolidayAlreadyExists.GetDescription());
            }
            
            return await base.UpdateAsync<TProjectTo>(model);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }
    
    /// <summary>
    /// Deletes holiday records with the specified summary asynchronously.
    /// </summary>
    /// <param name="summary">The summary of the holidays to delete.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure of the deletion.</returns>
    public async Task<Result> DeleteBySummaryAsync(string summary)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var result = await _repository.DeleteBySummaryAsync(summary);
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

    /// <summary>
    /// Deletes holiday records with the specified summary from a specific date onward asynchronously.
    /// </summary>
    /// <param name="summary">The summary of the holidays to delete.</param>
    /// <param name="date">The start date from which to delete holidays onward.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure of the deletion.</returns>
    public async Task<Result> DeleteBySummaryFromDateAsync(string summary, DateOnly date)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var result = await _repository.DeleteBySummaryFromDateAsync(summary, date);
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

    /// <summary>
    /// Deletes holiday records from a specific date onward asynchronously.
    /// </summary>
    /// <param name="date">The start date from which to delete holidays onward.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure of the deletion.</returns>
    public async Task<Result> DeleteFromDateAsync(DateOnly date)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var result = await _repository.DeleteFromDateAsync(date);
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

    /// <summary>
    /// Deletes holiday records with the specified date asynchronously.
    /// </summary>
    /// <param name="dateToDelete">The date of the holidays to delete.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure of the deletion.</returns>
    public async Task<Result> DeleteByDateAsync(DateOnly dateToDelete)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var result = await _repository.DeleteByDateAsync(dateToDelete);
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