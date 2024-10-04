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
/// Repository class for managing absences, including CRUD operations.
/// </summary>
public class AbsencesRepository : BaseCrudRepository<TimeTrackerDbContext, AbsenceEntity, Absence, int>,
    IAbsencesRepository
{
    /// <summary>
    /// Initializes a new instance of the repository with the specified context factory and mapper.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="contextFactory">The factory for creating database context instances.</param>
    /// <param name="mapper">The object mapper.</param>
    public AbsencesRepository(TimeTrackerDbContext context, IDbContextFactory<TimeTrackerDbContext> contextFactory,
        IMapper mapper)
        : base(context, contextFactory, mapper) { }

    /// <summary>
    /// Retrieves all absence records from the database.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the absence records should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all absences projected to the specified type.</returns>
    public async Task<IList<TProjectTo>> GetAllAsync<TProjectTo>() =>
        await _transientContext.Absences
            .TagWith(AbsenceQueryTags.GetAllAbsences.GetDescription())
            .AsNoTracking()
            .ProjectTo<TProjectTo>(_mapperConfig)
            .ToListAsync();

    /// <summary>
    /// Retrieves all absence records for a specific user identified by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose absences are to be retrieved.</param>
    /// <typeparam name="TProjectTo">The type to which the absence records should be projected.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of absences for the specified user, projected to the specified type.</returns>
    public async Task<IList<TProjectTo>> GetByUserIdAsync<TProjectTo>(string userId) =>
        await _transientContext.Absences
            .TagWith(AbsenceQueryTags.GetAbsencesByUserId.GetDescription())
            .AsNoTracking()
            .Where(card => card.UserId == userId)
            .ProjectTo<TProjectTo>(_mapperConfig)
            .ToListAsync();

    /// <summary>
    /// Toggles the status of an absence record by its ID.
    /// </summary>
    /// <param name="id">The ID of the absence record whose status is to be toggled.</param>
    /// <param name="statusOfType">Optional; the status of type to be set for the absence record.</param>
    /// <param name="statusOfDates">Optional; the status of dates to be set for the absence record.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result indicating whether the operation was successful.</returns>
    public async Task<Result<TProjectTo>> ToggleStatusById<TProjectTo>(int id, ConfirmationStatus? statusOfType = null,
        ConfirmationStatus? statusOfDates = null)
    {
        var absenceEntity = await _persistentContext.Absences.FirstOrDefaultAsync(a => a.Id == id);
        if (absenceEntity == null)
        {
            return Result.Failure<TProjectTo>(AbsencesRepositoryErrorMessages.AbsenceNotFound.GetDescription());
        }

        if (statusOfType != null)
        {
            absenceEntity.StatusOfType = statusOfType.Value;
        }

        if (statusOfDates != null)
        {
            absenceEntity.StatusOfDates = statusOfDates.Value;
        }
        
        if (absenceEntity is { StatusOfDates: ConfirmationStatus.Confirmed, StatusOfType: ConfirmationStatus.Confirmed })
        {
            absenceEntity.IsFullyConfirmed = ConfirmationStatus.Confirmed;
        }

        _persistentContext.Absences.Update(absenceEntity);
        var result = await SaveAsync(_persistentContext);

        return result.Value
            ? Result.Success(_mapper.Map<TProjectTo>(absenceEntity))
            : Result.Failure<TProjectTo>(AbsencesRepositoryErrorMessages.UpdatingFailed.GetDescription());
    }
}