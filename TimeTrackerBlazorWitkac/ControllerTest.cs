using Microsoft.AspNetCore.Mvc;
using TimeTrackerBlazorWitkac.Contracts.Requests;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Dto.UserCardDto;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Options;

namespace TimeTrackerBlazorWitkac;

[Route("api/test")]
[ApiController]
public class ControllerTest : ControllerBase
{
    private readonly IUserCardsService _userCardsService;
    private readonly IUsersRepository _usersRepository;
    private readonly IHolidaysService _holidaysService;
    private readonly IAttendancesService _attendancesService;
    private readonly IAbsencesService _absencesService;
    private readonly ICompaniesService _companiesService;


    public ControllerTest(
        IUserCardsService userCardsService,
        IUsersRepository usersRepository,
        IHolidaysService holidaysService,
        IAttendancesService attendancesService,
        IAbsencesService absencesService,
        ICompaniesService companiesService)

    {
        _userCardsService = userCardsService;
        _usersRepository = usersRepository;
        _holidaysService = holidaysService;
        _attendancesService = attendancesService;
        _absencesService = absencesService;
        _companiesService = companiesService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = "Hello from API" });
    }

    [HttpPost("user-card/create")]
    public async Task<IActionResult> CreateUserCard([FromBody] CreateUserCardRequest createUserCardRequest)
    {
        var result = UserCard.Builder()
            .SetUserId(createUserCardRequest.UserId)
            .SetCompanyId(createUserCardRequest.CompanyId)
            .SetNumber(createUserCardRequest.Number)
            .SetCardType(createUserCardRequest.CardType)
            .SetName(createUserCardRequest.Name)
            .Build();

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        var response = await _userCardsService.CreateAsync<UserCardResponse>(result.Value);
        if (response.IsFailure)
        {
            return BadRequest($"Something went wrong! Error: {response.Error}");
        }

        return Ok(response.Value);
    }

    [HttpPut("user-card/update")]
    public async Task<IActionResult> UpdateUserCard([FromBody] UpdateUserCardRequest editUserCardRequest,
        int userCardId)
    {
        var userCard = await _userCardsService.GetByIdAsync<UserCard>(userCardId);

        if (userCard.IsFailure)
        {
            return BadRequest(userCard.Error);
        }

        if (userCard.Value == null)
        {
            return BadRequest("UserCard does not exist");
        }

        var updatedUserCard = UserCard.Builder()
            .SetId(userCard.Value.Id)
            .SetUserId(userCard.Value.UserId)
            .SetCompanyId(userCard.Value.CompanyId)
            .SetNumber(userCard.Value.Number)
            .SetCardType(userCard.Value.CardType)
            .SetName(editUserCardRequest.Name)
            .Build();

        if (updatedUserCard.IsFailure)
        {
            return BadRequest($"UserCard update failed! {updatedUserCard.Error}");
        }

        var result = await _userCardsService.UpdateAsync<UserCard>(
            updatedUserCard.Value with { Id = userCard.Value.Id });

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("company/create")]
    public async Task<IActionResult> CreateUserCard([FromBody] CreateCompanyRequest request)
    {
        var company = Company.Builder()
            .SetName(request.Name)
            .Build();
        if (company.IsFailure)
        {
            return BadRequest(company.Error);
        }

        var result = await _companiesService.CreateAsync<Company>(company.Value);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("company/update")]
    public async Task<IActionResult> CreateUserCard([FromBody] UpdateCompanyRequest request)
    {
        var company = Company.Builder()
            .SetName(request.Name)
            .Build();
        if (company.IsFailure)
        {
            return BadRequest(company.Error);
        }

        var result = await _companiesService.UpdateAsync<Company>(company.Value);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete("company/delete-by-id")]
    public async Task<IActionResult> DeleteCompanyById(int id)
    {
        var result = await _companiesService.DeleteByIdAsync(id);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Success!");
    }

    [HttpGet("user-card/get-user-card-by-user")]
    public async Task<IActionResult> GetUserCardByUser(string userId)
    {
        var user = await _usersRepository.IsUserExistsById(userId);
        if (!user)
        {
            return BadRequest("User does not exist!");
        }

        var result = await _userCardsService
            .GetByUserAsync<UserCard>(userId);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("user-card/get-user-card-by-id")]
    public async Task<IActionResult> GetUserCardById(int userCardId)
    {
        var result = await _userCardsService
            .GetByIdAsync<UserCard>(userCardId);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        if (result.Value == null)
        {
            return BadRequest("UserCard doesn't exist!");
        }

        return Ok(result.Value);
    }

    [HttpGet("user-card/get-all-user-card")]
    public async Task<IActionResult> GetAllUserCard()
    {
        var result = await _userCardsService
            .GetAllAsync<UserCardDtoCompany>();

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("attendance/get-filtered")]
    public async Task<IActionResult> GetFiltered(int? cardId, string? userId, DateOnly? startDate, DateOnly? endDate)
    {
        var result = await _attendancesService.GetFilteredAsync<Attendance>(
            userCardId: cardId,
            userId: userId,
            startDate: startDate,
            endDate: endDate);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    //real card punch
    [HttpPost("attendance/card-punch")]
    public async Task<IActionResult> CardPunch(string number)
    {
        var result = await _attendancesService.CardPunchAsync<Attendance>(number, DateTime.Now);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    //card punch to test
    [HttpPost("attendance/card-punch-with-body")]
    public async Task<IActionResult> CardPunchWithBody([FromBody] CardPunchRequest request)
    {
        var result = await _attendancesService.CardPunchAsync<Attendance>(request.Number, request.DateTime);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("attendance/get-last-by-card-id")]
    public async Task<IActionResult> GetLastAttendanceByCardId(int cardId)
    {
        var result = await _attendancesService.GetLastByCardIdAsync<AttendanceResponse>(cardId);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        
        return Ok(result.Value);
    }

    [HttpPut("admin/attendance/update")]
    public async Task<IActionResult> AttendanceAdminUpdate([FromBody] AdminAttendanceUpdateRequest request)
    {
        var result = await _attendancesService.UpdateOrResolveAsync<Attendance>(request);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("admin/attendance/resolve")]
    public async Task<IActionResult> AttendanceAdminUpdate([FromBody] AdminAttendanceUpdateRequest request, bool forceResolve)
    {
        var result = await _attendancesService.UpdateOrResolveAsync<Attendance>(request, forceResolve);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("admin/attendance/create")]
    public async Task<IActionResult> AttendanceAdminCreate([FromBody] AdminAttendanceCreateRequest request)
    {
        var result = await _attendancesService.CreateAsync<Attendance>(request);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpDelete("admin/attendance/delete/{id}")]
    public async Task<IActionResult> AttendanceAdminCreate([FromRoute] int id)
    {
        var result = await _attendancesService.DeleteByIdAsync(id);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Success!!!");
    }

    [HttpGet("holiday/get-by-year-month")]
    public async Task<IActionResult> GetHolidaysByYearMonth(DateOnly date)
    {
        var result = await _holidaysService.GetByYearMonthAsync<Holiday>(date);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("holiday/get-by-date-with-summary")]
    public async Task<IActionResult> GetHolidaysByDateWithSummary(DateOnly date, string summary)
    {
        if (string.IsNullOrEmpty(summary))
        {
            return BadRequest("Summary cannot be null or empty!");
        }

        var result = await _holidaysService.GetByDateWithSummaryAsync<Holiday>(date, summary);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("holiday/get-all")]
    public async Task<IActionResult> GetAllHolidays()
    {
        var result = await _holidaysService.GetAllAsync<Holiday>();
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpPut("holiday/update/{holidayId}")]
    public async Task<IActionResult> UpdateHoliday([FromBody] UpdateHolidayRequest request, [FromRoute] int holidayId)
    {
        var holidayToUpdate = Holiday.Builder()
            .SetName(request.Summary)
            .SetStartDate(request.StartDate)
            .SetEndDate(request.EndDate)
            .SetDescription(request.Description)
            .Build();

        if (holidayToUpdate.IsFailure)
        {
            return BadRequest(holidayToUpdate.Error);
        }

        await _holidaysService.UpdateAsync<Holiday>(
            holidayToUpdate.Value with { Id = holidayId });

        return NoContent();
    }

    [HttpDelete("holiday/delete-by-summary")]
    public async Task<IActionResult> DeleteHolidaysBySummary(string summary)
    {
        if (string.IsNullOrWhiteSpace(summary))
        {
            return BadRequest("Summary cannot be empty or with white spaces!");
        }

        var result = await _holidaysService.DeleteBySummaryAsync(summary);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Success!");
    }

    [HttpDelete("holiday/delete-by-summary-from-date")]
    public async Task<IActionResult> DeleteBySummaryFromDateAsync(string summary, DateOnly date)
    {
        if (string.IsNullOrWhiteSpace(summary))
        {
            return BadRequest("Summary cannot be empty or with white spaces!");
        }

        var result = await _holidaysService.DeleteBySummaryFromDateAsync(summary, date);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Success!");
    }
    [HttpDelete("holiday/delete-from-date")]
    public async Task<IActionResult> DeleteFromDateAsync(DateOnly date)
    {
        var result = await _holidaysService.DeleteFromDateAsync(date);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Success!");
    }

    [HttpDelete("holiday/delete-by-date")]
    public async Task<IActionResult> DeleteByDateAsync(DateOnly dateToDelete)
    {
        var result = await _holidaysService.DeleteByDateAsync(dateToDelete);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Success!");
    }

    [HttpDelete("holiday/delete-by-id/{id}")]
    public async Task<IActionResult> DeleteByIdAsync([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid ID specified.");
        }

        var result = await _holidaysService.DeleteByIdAsync(id);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Success!");
    }

    [HttpPost("absence/create")]
    public async Task<IActionResult> AbsenceCreate([FromBody] CreateAbsenceRequest model)
    {
        var absence = Absence.Builder()
            .SetUserId(model.UserId)
            .SetAbsenceTypeId(model.AbsenceTypeId)
            .SetIsFullDate(model.IsFullDate)
            .SetStartDate(model.StartDate)
            .SetStartHour(model.StartHour)
            .SetStartMinute(model.StartMinute)
            .SetEndDate(model.EndDate)
            .SetEndHour(model.EndHour)
            .SetEndMinute(model.EndMinute)
            .SetReason(model.Reason)
            .Build();

        if (absence.IsFailure)
        {
            return BadRequest(absence.Error);
        }

        var result = await _absencesService.CreateAsync<Absence>(absence.Value);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPut("absence/update")]
    public async Task<IActionResult> AbsenceUpdateStatus([FromBody] AbsenceResponse model)
    {
        var absenceExist = await _absencesService.GetByIdAsync<Absence>(model.Id);

        if (absenceExist.IsFailure)
        {
            return BadRequest(absenceExist.Error);
        }

        if (absenceExist.Value == null)
        {
            return BadRequest("Absence not found!");
        }

        var absence = Absence.Builder()
            .SetId(absenceExist.Value.Id)
            .SetUserId(absenceExist.Value.UserId)
            .SetAbsenceTypeId(absenceExist.Value.AbsenceTypeId)
            .SetIsFullDate(model.IsFullDate)
            .SetStartDate(DateOnly.FromDateTime(model.StartDate))
            .SetStartHour(model.StartDate.Hour)
            .SetStartMinute(model.StartDate.Minute)
            .SetEndDate(DateOnly.FromDateTime(model.EndDate))
            .SetEndHour(model.EndDate.Hour)
            .SetEndMinute(model.EndDate.Minute)
            .SetReason(model.Reason)
            .Build();

        if (absence.IsFailure)
        {
            return BadRequest(absence.Error);
        }

        var result = await _absencesService.UpdateAsync<Absence>(absence.Value);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok("Success!");
    }

    [HttpPut("absence/toggle-status")]
    public async Task<IActionResult> AbsenceToggleStatusById(ConfirmationStatus? statusOfType, ConfirmationStatus? statusOfDates, int id)
    {
        var absenceExist = await _absencesService.GetByIdAsync<Absence>(id);

        if (absenceExist.IsFailure)
        {
            return BadRequest(absenceExist.Error);
        }

        if (absenceExist.Value == null)
        {
            return BadRequest("Absence not found!");
        }

        var result = await _absencesService.ToggleStatusById<WorkDayResponse>(id, statusOfType, statusOfDates);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Success!!!");
    }

    [HttpDelete("absence/delete")]
    public async Task<IActionResult> AbsenceDelete([FromBody] int id)
    {
        var result = await _absencesService.DeleteByIdAsync(id);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Success!");
    }
    [HttpGet("absence/get-all-by-user")]
    public async Task<IActionResult> GetAllAbsencesByUser(string userId)
    {
        var result = await _absencesService.GetByUserIdAsync<AbsenceResponse>(userId);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("user/get-pagination")]
    public async Task<IActionResult> GetPagination()
    {
        var result = await _usersRepository.GetFilteredPageAsync<User>(
            options: new PaginationOptions { Page = 1, PageSize = 10 },
            predicate: user => user.Email!.Contains(""),
            orderBy: query => query.OrderBy(user => user.Email),
            cancellationToken: CancellationToken.None);


        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("workday/get-mapped-from-absence")]
    public async Task<IActionResult> GetMappedFromAbsenceAsync()
    {
        var result = await _absencesService.GetAllAsync<WorkDayResponse>();
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("workday/get-mapped-from-attendance")]
    public async Task<IActionResult> GetMappedFromAttendanceAsync()
    {
        var result = await _attendancesService.GetFilteredAsync<WorkDayResponse>();

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
