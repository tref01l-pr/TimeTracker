using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Interfaces.Repository;

/// <summary>
/// Defines the contract for managing Holiday entities within the repository.
/// </summary>
public interface IHolidaysRepository : ICrudRepository<HolidayEntity, Holiday, int>
{
    /// <summary>
    /// Retrieves all Holiday records and projects them to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of projected Holiday records.</returns>
    Task<IList<TProjectTo>> GetAllAsync<TProjectTo>();

    /// <summary>
    /// Retrieves Holiday records that fall within the specified date range and projects them to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of projected Holiday records within the specified date range.</returns>
    Task<IList<TProjectTo>> GetByDates<TProjectTo>(DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Retrieves Holiday records for a specific year and month, and projects them to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holidays to.</typeparam>
    /// <param name="date">The date specifying the year and month.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of projected Holiday records for the specified year and month.</returns>
    Task<IList<TProjectTo>> GetByYearMonthAsync<TProjectTo>(DateOnly date);

    /// <summary>
    /// Retrieves a Holiday record by its start date and summary, and projects it to the specified type.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to project the holiday to.</typeparam>
    /// <param name="date">The start date of the holiday.</param>
    /// <param name="summary">The summary of the holiday.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the projected Holiday record, or null if not found.</returns>
    Task<TProjectTo?> GetByDateWithSummaryAsync<TProjectTo>(DateOnly date, string summary);

    /// <summary>
    /// Deletes Holiday records by their summary.
    /// </summary>
    /// <param name="summary">The summary of the holidays to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> DeleteBySummaryAsync(string summary);

    /// <summary>
    /// Deletes Holiday records by their summary and a specified start date.
    /// </summary>
    /// <param name="summary">The summary of the holidays to delete.</param>
    /// <param name="date">The start date from which to delete holidays.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> DeleteBySummaryFromDateAsync(string summary, DateOnly date);

    /// <summary>
    /// Deletes Holiday records from a specific start date.
    /// </summary>
    /// <param name="date">The start date from which to delete holidays.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> DeleteFromDateAsync(DateOnly date);

    /// <summary>
    /// Deletes Holiday records by their start date.
    /// </summary>
    /// <param name="dateToDelete">The start date of the holidays to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> DeleteByDateAsync(DateOnly dateToDelete);
}