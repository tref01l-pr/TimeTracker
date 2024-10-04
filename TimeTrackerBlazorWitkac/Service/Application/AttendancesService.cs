using System.Linq.Expressions;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services;
using CSharpFunctionalExtensions;
using TimeTrackerBlazorWitkac.AlertMessages.ErrorMessages.Services.Application;
using TimeTrackerBlazorWitkac.Contracts.Requests;
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
/// Service for managing attendance records, including CRUD operations and specific attendance-related functionalities.
/// </summary>
public class AttendancesService : CrudService<IAttendancesRepository, AttendanceEntity, Attendance, int>,
    IAttendancesService
{
    private readonly IAttendancesRepository _attendancesRepository;
    private readonly IUserCardsRepository _userCardsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IAttendanceChecker _attendanceChecker;
    private readonly ICompaniesRepository _companiesRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttendancesService"/> class.
    /// </summary>
    /// <param name="attendancesRepository">Repository for attendance records.</param>
    /// <param name="transactionRepository">Repository for transaction management.</param>
    /// <param name="userCardsRepository">Repository for user card management.</param>
    /// <param name="usersRepository">Repository for user management.</param>
    /// <param name="companiesRepository">Repository for company management.</param>
    /// <param name="attendanceChecker">Service for checking attendance-related rules and validations.</param>
    public AttendancesService(
        IAttendancesRepository attendancesRepository,
        ITransactionRepository transactionRepository,
        IUserCardsRepository userCardsRepository,
        IUsersRepository usersRepository,
        ICompaniesRepository companiesRepository,
        IAttendanceChecker attendanceChecker) : base(attendancesRepository, transactionRepository)
    {
        _attendancesRepository = attendancesRepository;
        _userCardsRepository = userCardsRepository;
        _usersRepository = usersRepository;
        _companiesRepository = companiesRepository;
        _attendanceChecker = attendanceChecker;
    }

    /// <summary>
    /// Retrieves a filtered list of attendance records based on specified criteria.
    /// </summary>
    /// <param name="options">Pagination options.</param>
    /// <param name="userCardId">Optional user card ID for filtering.</param>
    /// <param name="userId">Optional user ID for filtering.</param>
    /// <param name="startDate">Optional start date for filtering.</param>
    /// <param name="endDate">Optional end date for filtering.</param>
    /// <param name="idToIgnore">Optional ID to ignore in the filtering.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A paginated list of filtered attendance records.</returns>
    public async Task<Result<PageResponse<TProjectTo>>> GetFilteredAsync<TProjectTo>(
        PaginationOptions? options = null,
        int? userCardId = null,
        string? userId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        int? idToIgnore = null,
        CancellationToken cancellationToken = default)
    {
        IList<Expression<Func<AttendanceEntity, bool>>> predicates =
            new List<Expression<Func<AttendanceEntity, bool>>>();

        if (userCardId.HasValue)
        {
            predicates.Add(a => a.UserCardId == userCardId.Value);
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

        if (idToIgnore.HasValue)
        {
            predicates.Add(a => a.Id != idToIgnore.Value);
        }

        return await _attendancesRepository.GetFilteredPageAsync<TProjectTo>(
            options,
            PredicateCombiner.Combine<AttendanceEntity>(predicates.ToArray()),
            orderBy: query => query.OrderByDescending(a => a.StartDate)
                .ThenByDescending(a => a.StartHour)
                .ThenByDescending(a => a.StartMinute),
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves the last attendance record associated with a specific user card ID.
    /// </summary>
    /// <param name="userCardId">The user card ID to retrieve the last attendance for.</param>
    /// <returns>The last attendance record or a failure result if not found.</returns>
    public async Task<Result<TProjectTo?>> GetLastByCardIdAsync<TProjectTo>(int userCardId) where TProjectTo : class
    {
        try
        {
            var result = await _repository.GetLastByCardIdAsync<TProjectTo>(userCardId);
            return Result.Success(result);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo?>(e.Message);
        }
    }

    /// <summary>
    /// Processes a card punch operation, either opening or closing an attendance record based on the current state.
    /// </summary>
    /// <param name="number">The number associated with the user card.</param>
    /// <param name="actionTime"></param>
    /// <returns>A result containing the new attendance record or an error message.</returns>
    public async Task<Result<TProjectTo>> CardPunchAsync<TProjectTo>(string number, DateTime actionTime)
    {
        var userCard = await _userCardsRepository.GetByNumberAsync<UserCard>(number);
        if (userCard == null)
        {
            return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.UserCardNotFound.GetDescription());
        }

        var lastAttendance = await _attendancesRepository.GetLastByCardIdAsync<Attendance>(userCard.Id);

        return lastAttendance is not { EndDate: null }
            ? await OpenAttendanceAsync<TProjectTo>(userCard, actionTime)
            : await CloseAttendanceIfEligibleAsync<TProjectTo>(lastAttendance, userCard, actionTime);
    }

    /// <summary>
    /// Updates or resolves an attendance record based on the provided update request and admin permissions.
    /// </summary>
    /// <param name="attendanceUpdateRequest">The request containing the update details.</param>
    /// <param name="forceResolve">Indicates whether to force resolution of strange activity.</param>
    /// <returns>A result containing the updated attendance record or an error message.</returns>
    public async Task<Result<TProjectTo>> UpdateOrResolveAsync<TProjectTo>(
        AdminAttendanceUpdateRequest attendanceUpdateRequest, bool forceResolve = false)
    {
        try
        {
            //Can be used only by Admin!
            var userExist = await _usersRepository.IsUserExistsById(attendanceUpdateRequest.AdminId);
            if (!userExist)
            {
                return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.AdminIdNotFound.GetDescription());
            }

            var attendance =
                await _attendancesRepository.GetByIdAsync<Attendance>(attendanceUpdateRequest.AttendanceId);
            if (attendance == null)
            {
                return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.AttendanceNotFound.GetDescription());
            }

            UserCard? userCard = null;
            if (attendance.UserCardId != attendanceUpdateRequest.UserCardId)
            {
                userCard = await _userCardsRepository.GetByIdAsync<UserCard>(attendanceUpdateRequest.UserCardId);
                if (userCard == null)
                {
                    return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.UserCardNotFound
                        .GetDescription());
                }
            }

            bool hasCollision = await _attendanceChecker.CheckDateCollision(
                attendanceUpdateRequest.UserCardId,
                attendanceUpdateRequest.StartDate,
                attendanceUpdateRequest.StartHour,
                attendanceUpdateRequest.StartMinute,
                attendanceUpdateRequest.EndDate,
                attendanceUpdateRequest.EndHour,
                attendanceUpdateRequest.EndMinute,
                attendanceUpdateRequest.AttendanceId);

            if (hasCollision)
            {
                return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.AttendanceCollision.GetDescription());
            }

            var strangeActivityResult = forceResolve
                ? Result.Success()
                : await _attendanceChecker.CheckStrangeActivity(
                    attendanceUpdateRequest.UserCardId,
                    attendanceUpdateRequest.StartDate,
                    attendanceUpdateRequest.EndDate,
                    new TimeSpan(attendanceUpdateRequest.StartHour, attendanceUpdateRequest.StartMinute, 0),
                    new TimeSpan(attendanceUpdateRequest.EndHour, attendanceUpdateRequest.EndMinute, 0),
                    attendanceUpdateRequest.AttendanceId);

            var builder = Attendance.Builder()
                .SetId(attendance.Id)
                .SetUserCardId(userCard?.Id ?? attendance.UserCardId)
                .SetUserId(userCard?.UserId ?? attendance.UserId)
                .SetCompanyId(userCard?.CompanyId ?? attendance.CompanyId)
                .SetStartDate(attendanceUpdateRequest.StartDate)
                .SetStartHour(attendanceUpdateRequest.StartHour)
                .SetStartMinute(attendanceUpdateRequest.StartMinute)
                .SetEndDate(attendanceUpdateRequest.EndDate)
                .SetEndHour(attendanceUpdateRequest.EndHour)
                .SetEndMinute(attendanceUpdateRequest.EndMinute)
                .SetIsStrangeActivity(strangeActivityResult.IsFailure)
                .SetStrangeActivityReason(strangeActivityResult.IsFailure ? strangeActivityResult.Error : null);

            if (attendance.IsStrangeActivity && !strangeActivityResult.IsFailure)
            {
                builder.SetResolvedAt(DateTime.Now).SetResolvedById(attendanceUpdateRequest.AdminId);
            }

            var attendanceToUpdate = builder.Build();
            if (attendanceToUpdate.IsFailure)
            {
                return Result.Failure<TProjectTo>(attendanceToUpdate.Error);
            }

            return await base.UpdateAsync<TProjectTo>(attendanceToUpdate.Value);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    /// <summary>
    /// Overrides the update method to prevent updating attendance records.
    /// </summary>
    /// <typeparam name="TProjectTo">The type of the project to return.</typeparam>
    /// <param name="model">The attendance model to update.</param>
    /// <returns>A result indicating the failure of the update operation.</returns>
    public override Task<Result<TProjectTo>> UpdateAsync<TProjectTo>(Attendance model)
    {
        return Task.FromResult(
            Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.AttendanceInvalidMethodUpdate.GetDescription()));
    }

    /// <summary>
    /// Deletes an attendance record by its ID. This operation can only be performed by an admin.
    /// </summary>
    /// <param name="id">The ID of the attendance record to delete.</param>
    /// <returns>A result indicating the outcome of the delete operation.</returns>
    public override Task<Result> DeleteByIdAsync(int id)
    {
        //Can be Deleted only by Admin!
        return base.DeleteByIdAsync(id);
    }

    /// <summary>
    /// Creates a new attendance record. This operation can only be called by an admin.
    /// </summary>
    /// <typeparam name="TProjectTo">The type of the project to return.</typeparam>
    /// <param name="attendance">The attendance record to create.</param>
    /// <returns>A result containing the created attendance record or an error message.</returns>
    public override async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(Attendance attendance)
    {
        try
        {
            //Can be called only by Admin!
            var userCard = await _userCardsRepository.GetByIdAsync<UserCard>(attendance.UserCardId);
            if (userCard == null)
            {
                return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.UserCardNotFound.GetDescription());
            }

            var user = await _usersRepository.GetByIdAsync<User>(attendance.UserId);
            if (user == null)
            {
                return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.UserNotFound.GetDescription());
            }

            var company = await _companiesRepository.GetByIdAsync<Company>(attendance.CompanyId);
            if (company == null)
            {
                return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.UserNotFound.GetDescription());
            }

            bool hasCollision = await _attendanceChecker.CheckDateCollision(
                attendance.UserCardId,
                attendance.StartDate,
                attendance.StartHour,
                attendance.StartMinute,
                attendance.EndDate,
                attendance.EndHour,
                attendance.EndMinute);

            if (hasCollision)
            {
                return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.AttendanceCollision.GetDescription());
            }

            return await base.CreateAsync<TProjectTo>(attendance);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    /// <summary>
    /// Creates an attendance record from a user card number. This method is intended for admin use only.
    /// </summary>
    /// <typeparam name="TProjectTo">The type of the project to return.</typeparam>
    /// <param name="attendanceCreateRequest">The request containing details for the attendance creation.</param>
    /// <returns>A result containing the created attendance record or an error message.</returns>
    public async Task<Result<TProjectTo>> CreateAsync<TProjectTo>(
        AdminAttendanceCreateRequest attendanceCreateRequest)
    {
        var userCard = await _userCardsRepository.GetByIdAsync<UserCard>(attendanceCreateRequest.UserCardId);
        if (userCard == null)
        {
            return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.UserCardNotFound.GetDescription());
        }

        bool hasCollision = await _attendanceChecker.CheckDateCollision(
            attendanceCreateRequest.UserCardId,
            attendanceCreateRequest.StartDate,
            attendanceCreateRequest.StartHour,
            attendanceCreateRequest.StartMinute,
            attendanceCreateRequest.EndDate,
            attendanceCreateRequest.EndHour,
            attendanceCreateRequest.EndMinute);

        if (hasCollision)
        {
            return Result.Failure<TProjectTo>(AttendancesServiceErrorMessages.AttendanceCollision.GetDescription());
        }

        var attendance = Attendance.Builder()
            .SetUserCardId(userCard.Id)
            .SetUserId(userCard.UserId)
            .SetCompanyId(userCard.CompanyId)
            .SetStartDate(attendanceCreateRequest.StartDate)
            .SetStartHour(attendanceCreateRequest.StartHour)
            .SetStartMinute(attendanceCreateRequest.StartMinute)
            .SetEndDate(attendanceCreateRequest.EndDate)
            .SetEndHour(attendanceCreateRequest.EndHour)
            .SetEndMinute(attendanceCreateRequest.EndMinute)
            .Build();

        if (attendance.IsFailure)
        {
            return Result.Failure<TProjectTo>(attendance.Error);
        }

        var result = await _attendancesRepository.CreateAsync<TProjectTo>(attendance.Value);

        return result.IsFailure
            ? Result.Failure<TProjectTo>(result.Error)
            : result.Value;
    }
    
    /// <summary>
    /// Opens a new attendance record for the specified user card.
    /// </summary>
    /// <param name="userCard">The user card to open attendance for.</param>
    /// <param name="actionTime"></param>
    /// <returns>A result containing the newly created attendance record or an error message.</returns>
    private async Task<Result<TProjectTo>> OpenAttendanceAsync<TProjectTo>(UserCard userCard, DateTime actionTime)
    {
        try
        {
            var attendance = Attendance.Builder()
                .SetUserCardId(userCard.Id)
                .SetUserId(userCard.UserId)
                .SetCompanyId(userCard.CompanyId)
                .SetStartDate(DateOnly.FromDateTime(actionTime))
                .SetStartHour(actionTime.Hour)
                .SetStartMinute(actionTime.Minute)
                .Build();

            if (attendance.IsFailure)
            {
                return Result.Failure<TProjectTo>(attendance.Error);
            }

            return await base.CreateAsync<TProjectTo>(attendance.Value);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    private async Task<Result<TProjectTo>> CloseAttendanceIfEligibleAsync<TProjectTo>(Attendance attendanceToClose,
        UserCard userCard, DateTime actionTime)
    {
        var openTime = new DateTime(attendanceToClose.StartDate,
            new TimeOnly(attendanceToClose.StartHour, attendanceToClose.StartMinute, 0));
        var timeDifference = actionTime - openTime;
        if (timeDifference.TotalHours > Attendance.MaxDailyWorkHours)
        {
            return await EndCurrentAttendanceAndBeginNew<TProjectTo>(attendanceToClose, userCard, actionTime);
        }

        return await CloseAttendanceAsync<TProjectTo>(attendanceToClose, actionTime);
    }

    /// <summary>
    /// Closes an existing attendance record and updates it with the end time and any associated strange activity checks.
    /// </summary>
    /// <param name="attendanceToClose">The attendance record to close.</param>
    /// <param name="actionTime"></param>
    /// <returns>A result containing the updated attendance record or an error message.</returns>
    private async Task<Result<TProjectTo>> CloseAttendanceAsync<TProjectTo>(Attendance attendanceToClose,
        DateTime actionTime)
    {
        try
        {
            var strangeActivityResult = await _attendanceChecker.CheckStrangeActivity(
                attendanceToClose.UserCardId,
                attendanceToClose.StartDate,
                DateOnly.FromDateTime(actionTime),
                new TimeSpan(attendanceToClose.StartHour, attendanceToClose.StartMinute, 0),
                new TimeSpan(actionTime.Hour, actionTime.Minute, 0),
                attendanceToClose.Id);

            //TODO Can make notifications for user and admins here

            var attendanceToUpdate = Attendance.Builder()
                .SetId(attendanceToClose.Id)
                .SetUserCardId(attendanceToClose.UserCardId)
                .SetUserId(attendanceToClose.UserId)
                .SetCompanyId(attendanceToClose.CompanyId)
                .SetStartDate(attendanceToClose.StartDate)
                .SetStartHour(attendanceToClose.StartHour)
                .SetStartMinute(attendanceToClose.StartMinute)
                .SetEndDate(DateOnly.FromDateTime(actionTime))
                .SetEndHour(actionTime.Hour)
                .SetEndMinute(actionTime.Minute)
                .SetIsStrangeActivity(strangeActivityResult.IsFailure)
                .SetStrangeActivityReason(strangeActivityResult.IsFailure ? strangeActivityResult.Error : null)
                .Build();

            if (attendanceToUpdate.IsFailure)
            {
                return Result.Failure<TProjectTo>(attendanceToUpdate.Error);
            }

            return await base.UpdateAsync<TProjectTo>(attendanceToUpdate.Value);
        }
        catch (Exception e)
        {
            return Result.Failure<TProjectTo>(e.Message);
        }
    }

    private async Task<Result<TProjectTo>> EndCurrentAttendanceAndBeginNew<TProjectTo>(Attendance attendanceToClose,
        UserCard userCard, DateTime actionTime)
    {
        var closeResult = await CloseAttendanceAsync<TProjectTo>(attendanceToClose, actionTime);
        if (closeResult.IsFailure)
        {
            return closeResult;
        }

        var openResult = await OpenAttendanceAsync<TProjectTo>(userCard, actionTime);
        return openResult;
    }
}
