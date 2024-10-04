using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Repositories;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Data.Repositories.BaseRepositories;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.QueryTags;

namespace TimeTrackerBlazorWitkac.Data.Repositories;

/// <summary>
/// Repository class for managing Holiday entities.
/// </summary>
public class HolidaysRepository : BaseCrudRepository<TimeTrackerDbContext, HolidayEntity, Holiday, int>, IHolidaysRepository
{
    /// <summary>
    /// Initializes a new instance of the repository with the specified context and mapper.
    /// </summary>
    /// <param name="context">The database context used for data operations.</param>
    /// <param name="contextFactory">The factory for creating database context instances.</param>
    /// <param name="mapper">The object mapper for entity-to-model conversion.</param>
    public HolidaysRepository(
        TimeTrackerDbContext context, 
        IDbContextFactory<TimeTrackerDbContext> contextFactory,
        IMapper mapper) : base(context, contextFactory, mapper) { }


    /// <summary>
    /// Retrieves all Holiday records and projects them to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <returns>A list of projected Holiday records.</returns>
    public async Task<IList<TProjectTo>> GetAllAsync<TProjectTo>()
    {
        return await _transientContext.Holidays
             .TagWith(HolidayQueryTags.GetAllHolidays.GetDescription())
             .AsNoTracking()
             .ProjectTo<TProjectTo>(_mapperConfig)
             .ToListAsync();
    }
    
    /// <summary>
    /// Retrieves Holiday records that fall within the specified date range and projects them to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A list of projected Holiday records within the specified date range.</returns>
    public async Task<IList<TProjectTo>> GetByDates<TProjectTo>(DateOnly startDate, DateOnly endDate)
    {
        return await _transientContext.Holidays
            .TagWith(HolidayQueryTags.GetHolidaysByDates.GetDescription())
            .AsNoTracking()
            .Where(h => h.StartDate <= endDate && h.EndDate >= startDate)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves Holiday records for a specific year and month, and projects them to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <param name="date">The date specifying the year and month.</param>
    /// <returns>A list of projected Holiday records for the specified year and month.</returns>
    public async Task<IList<TProjectTo>> GetByYearMonthAsync<TProjectTo>(DateOnly date)
    {
        return  await _transientContext.Holidays
            .TagWith(HolidayQueryTags.GetHolidaysByYearMonth.GetDescription())
            .AsNoTracking()
            .Where(h => h.StartDate.Year == date.Year && h.StartDate.Month == date.Month)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a Holiday record by its start date and summary, and projects it to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holiday to.</typeparam>
    /// <param name="date">The start date of the holiday.</param>
    /// <param name="summary">The summary of the holiday.</param>
    /// <returns>The projected Holiday record, or null if not found.</returns>
    public async Task<TProjectTo?> GetByDateWithSummaryAsync<TProjectTo>(DateOnly date, string summary)
    {
        return await _transientContext.Holidays
            .TagWith(HolidayQueryTags.GetHolidayByDateWithSummary.GetDescription())
            .AsNoTracking()
            .Where(h => h.Name == summary && h.StartDate == date)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Creates a new Holiday record if it does not already exist.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the created holiday to.</typeparam>
    /// <param name="model">The Holiday model to create.</param>
    /// <returns>A Result indicating success or failure, along with the projected holiday.</returns>
    public override async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(Holiday model)
    {
        Holiday? existingHoliday = await GetByDateWithSummaryAsync<Holiday>(model.StartDate, model.Name);
        if (existingHoliday != null)
        {
            return Result.Failure<TProjectTo>(HolidaysRepositoryErrorMessages.HolidayAlreadyExists.GetDescription());
        }
        
        return await base.CreateAsync<TProjectTo>(model);
    }

    /// <summary>
    /// Deletes Holiday records with the specified summary.
    /// </summary>
    /// <param name="summary">The summary of the holidays to delete.</param>
    /// <returns>A Result indicating whether the deletion was successful or contains an error message.</returns>
    public Task<Result> DeleteBySummaryAsync(string summary) => 
        DeleteAsync(h => h.Name == summary);

    /// <summary>
    /// Deletes Holiday records with the specified summary and start date from the given date onward.
    /// </summary>
    /// <param name="summary">The summary of the holidays to delete.</param>
    /// <param name="date">The start date from which to delete holidays onward.</param>
    /// <returns>A Result indicating whether the deletion was successful or contains an error message.</returns>
    public Task<Result> DeleteBySummaryFromDateAsync(string summary, DateOnly date) =>
        DeleteAsync(h => h.Name == summary && h.StartDate >= date);

    /// <summary>
    /// Deletes all Holiday records from the specified date onward.
    /// </summary>
    /// <param name="date">The start date from which to delete holidays onward.</param>
    /// <returns>A Result indicating whether the deletion was successful or contains an error message.</returns>
    public Task<Result> DeleteFromDateAsync(DateOnly date) => 
        DeleteAsync(h => h.StartDate >= date);

    /// <summary>
    /// Deletes Holiday records with the specified start date.
    /// </summary>
    /// <param name="dateToDelete">The start date of the holidays to delete.</param>
    /// <returns>A Result indicating whether the deletion was successful or contains an error message.</returns>
    public Task<Result> DeleteByDateAsync(DateOnly dateToDelete) =>
        DeleteAsync(h => h.StartDate == dateToDelete);
}