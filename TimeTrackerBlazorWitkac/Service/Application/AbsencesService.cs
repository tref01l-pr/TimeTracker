using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Helpers;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Options;
using TimeTrackerBlazorWitkac.Service.Application.BaseServices;

namespace TimeTrackerBlazorWitkac.Service.Application;

/// <summary>
/// Service class for managing absences, including CRUD operations and additional business logic.
/// </summary>
public class AbsencesService : CrudService<IAbsencesRepository, AbsenceEntity, Absence, int>, IAbsencesService
{
    private readonly IAbsenceTypesRepository _absenceTypesRepository;
    private readonly IUsersRepository _usersRepository;

    /// <summary>
    /// Initializes a new instance of the AbsencesService class.
    /// </summary>
    /// <param name="repository">The repository for managing absence records.</param>
    /// <param name="usersRepository">The repository for managing user records.</param>
    /// <param name="absenceTypesRepository">The repository for managing absence types.</param>
    /// <param name="transactionRepository">The repository for managing transactions.</param>
    public AbsencesService(
        IAbsencesRepository repository,
        IUsersRepository usersRepository,
        IAbsenceTypesRepository absenceTypesRepository,
        ITransactionRepository transactionRepository) : base(repository, transactionRepository)
    {
        _absenceTypesRepository = absenceTypesRepository;
        _usersRepository = usersRepository;
    }
    
    /// <summary>
    /// Retrieves all absence records from the database.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the absence records should be projected.</typeparam>
    /// <returns>A Result containing a list of all absence records.</returns>
    public async Task<Result<IList<TProjectTo>>> GetAllAsync<TProjectTo>()
    {
        try
        {
            return Result.Success(await _repository.GetAllAsync<TProjectTo>());
        }
        catch (Exception e)
        {
            return Result.Failure<IList<TProjectTo>>(e.Message);
        }
    }

    /// <summary>
    /// Retrieves all absence records for a specific user identified by their ID.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the absence records should be projected.</typeparam>
    /// <param name="userId">The ID of the user whose absences are to be retrieved.</param>
    /// <returns>A Result containing a list of absence records for the specified user.</returns>
    public async Task<Result<IList<TProjectTo>>> GetByUserIdAsync<TProjectTo>(string userId)
    {
        try
        {
            return Result.Success(await _repository.GetByUserIdAsync<TProjectTo>(userId));
        }
        catch (Exception e)
        {
            return Result.Failure<IList<TProjectTo>>(e.Message);
        }
    }

    /// <summary>
    /// Retrieves a filtered list of absence records based on specified criteria.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the absence records should be projected.</typeparam>
    /// <param name="options">Optional; pagination options for the result set.</param>
    /// <param name="id">Optional; the ID of the absence record to filter by.</param>
    /// <param name="userId">Optional; the ID of the user to filter absences by.</param>
    /// <param name="startDate">Optional; the start date for filtering absences.</param>
    /// <param name="endDate">Optional; the end date for filtering absences.</param>
    /// <param name="cancellationToken">Optional; a cancellation token to cancel the operation.</param>
    /// <returns>A Result containing a paginated response of absence records filtered by the specified criteria.</returns>
    public async Task<Result<PageResponse<TProjectTo>>> GetFilteredAsync<TProjectTo>(
        PaginationOptions? options = null,
        int? id = null,
        string? userId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IList<Expression<Func<AbsenceEntity, bool>>> predicates = new List<Expression<Func<AbsenceEntity, bool>>>();

            if (id.HasValue)
            {
                predicates.Add(a => a.Id == id.Value);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                predicates.Add(a => a.UserId == userId);
            }

            if (startDate.HasValue)
            {
                predicates.Add(a => a.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                predicates.Add(a => a.EndDate <= endDate.Value);
            }
        
            return await _repository.GetFilteredPageAsync<TProjectTo>(
                options,
                PredicateCombiner.Combine(predicates.ToArray()),
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            return Result.Failure<PageResponse<TProjectTo>>(e.Message);
        }
    }

    /// <summary>
    /// Toggles the status of an absence record by its ID.
    /// </summary>
    /// <param name="id">The ID of the absence record whose status is to be toggled.</param>
    /// <param name="statusOfType">Optional; the status of type to be set for the absence record.</param>
    /// <param name="statusOfDates">Optional; the status of dates to be set for the absence record.</param>
    /// <returns>A Result indicating whether the operation was successful.</returns>
    public async Task<Result<TProjectTo>> ToggleStatusById<TProjectTo>(int id, ConfirmationStatus? statusOfType = null, ConfirmationStatus? statusOfDates = null)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var result = await _repository.ToggleStatusById<TProjectTo>(id, statusOfType, statusOfDates);
            if (result.IsFailure)
            {
                throw new Exception(result.Error);
            }

            await _transactionRepository.CommitTransactionAsync(transaction);
            return result;
        }
        catch (Exception e)
        {
            await _transactionRepository.RollbackTransactionAsync(transaction);
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    /// <summary>
    /// Creates a new absence record.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the absence record should be projected.</typeparam>
    /// <param name="model">The absence model to be created.</param>
    /// <returns>A Result containing the created absence record projected to the specified type.</returns>
    public override async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(Absence model)
    {
        try
        {
            var user = await _usersRepository.GetByIdAsync<User>(model.UserId);

            if (user == null)
            {
                throw new Exception(AbsencesServiceErrorMessages.UserNotFound.GetDescription());
            }

            var absenceType = await _absenceTypesRepository.GetByIdAsync<AbsenceType>(model.AbsenceTypeId);
            if (absenceType == null)
            {
                throw new Exception(AbsencesServiceErrorMessages.AbsenceTypeNotFound.GetDescription());
            }
        
            return await base.CreateAsync<TProjectTo>(model);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    /// <summary>
    /// Updates an existing absence record.
    /// </summary>
    /// <typeparam name="TProjectTo">The type to which the absence record should be projected.</typeparam>
    /// <param name="model">The absence model with updated information.</param>
    /// <returns>A Result containing the updated absence record projected to the specified type.</returns>
    public override async Task<Result<TProjectTo>> UpdateAsync<TProjectTo>(Absence model)
    {
        try
        {
            var absence = await _repository.GetByIdAsync<Absence>(model.Id);
            
            if (absence == null)
            {
                throw new Exception(AbsencesServiceErrorMessages.AbsenceNotFound.GetDescription());
            }

            if (absence.UserId != model.UserId)
            {
                throw new Exception(AbsencesServiceErrorMessages.UserIdMismatch.GetDescription());
            }
            
            return await base.UpdateAsync<TProjectTo>(model);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }
}