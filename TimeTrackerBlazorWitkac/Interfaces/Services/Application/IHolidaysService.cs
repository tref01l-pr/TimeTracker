using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Services.Application;

/// <summary>
/// Defines the contract for managing Holiday entities within the service layer.
/// </summary>
public interface IHolidaysService : ICrudService<HolidayEntity, Holiday, int>
{
    /// <summary>
    /// Retrieves all Holiday records and projects them to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <returns>A Result containing a list of projected Holiday records.</returns>
    Task<Result<IList<TProjectTo>>> GetAllAsync<TProjectTo>();

    /// <summary>
    /// Retrieves Holiday records for a specific year and month, and projects them to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <param name="date">The date specifying the year and month.</param>
    /// <returns>A Result containing a list of projected Holiday records for the specified year and month.</returns>
    Task<Result<IList<TProjectTo>>> GetByYearMonthAsync<TProjectTo>(DateOnly date);

    /// <summary>
    /// Retrieves Holiday records that fall within the specified date range and projects them to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A Result containing a list of projected Holiday records within the specified date range.</returns>
    Task<Result<IList<TProjectTo>>> GetByDates<TProjectTo>(DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Retrieves a Holiday record by its start date and summary, and projects it to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holiday to.</typeparam>
    /// <param name="date">The start date of the holiday.</param>
    /// <param name="summary">The summary of the holiday.</param>
    /// <returns>A Result containing the projected Holiday record, or null if not found.</returns>
    Task<Result<TProjectTo?>> GetByDateWithSummaryAsync<TProjectTo>(DateOnly date, string summary);

    /// <summary>
    /// Creates new Holiday records.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the created holidays to.</typeparam>
    /// <param name="holidays">The array of Holiday models to create.</param>
    /// <returns>A Result containing a list of projected newly created Holiday records.</returns>
    Task<Result<IList<TProjectTo>>> CreateAsync<TProjectTo>(params Holiday[] holidays);

    /// <summary>
    /// Deletes Holiday records with the specified summary.
    /// </summary>
    /// <param name="summary">The summary of the holidays to delete.</param>
    /// <returns>A Result indicating whether the deletion was successful or contains an error message.</returns>
    Task<Result> DeleteBySummaryAsync(string summary);

    /// <summary>
    /// Deletes Holiday records with the specified summary and start date from the given date onward.
    /// </summary>
    /// <param name="summary">The summary of the holidays to delete.</param>
    /// <param name="date">The start date from which to delete holidays onward.</param>
    /// <returns>A Result indicating whether the deletion was successful or contains an error message.</returns>
    Task<Result> DeleteBySummaryFromDateAsync(string summary, DateOnly date);

    /// <summary>
    /// Deletes all Holiday records from the specified date onward.
    /// </summary>
    /// <param name="date">The start date from which to delete holidays onward.</param>
    /// <returns>A Result indicating whether the deletion was successful or contains an error message.</returns>
    Task<Result> DeleteFromDateAsync(DateOnly date);

    /// <summary>
    /// Deletes Holiday records with the specified start date.
    /// </summary>
    /// <param name="dateToDelete">The start date of the holidays to delete.</param>
    /// <returns>A Result indicating whether the deletion was successful or contains an error message.</returns>
    Task<Result> DeleteByDateAsync(DateOnly dateToDelete);
}