using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TimeTrackerBlazorWitkac.Data;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;

namespace TimeTrackerBlazorWitkac;

public class Seed(
    IDbContextFactory<TimeTrackerDbContext> contextFactory,
    RoleManager<RoleEntity> roleManager,
    UserManager<UserEntity> userManager,
    ICompaniesService companiesService,
    IAbsenceTypesService absenceTypesService,
    IHolidaysService holidaysService)
{
    //can be max 50
    private static int _yearsRange = 1;

    public async Task SeedDataContextAsync()
    {
        try
        {
            var adminEmail = "admin@mail.com";
            var memberEmail = "member@mail.com";
            var pass = "Pass1234!";

            var companyList = new List<Company>()
            {
                Company.Builder().
                    SetName("Infocity")
                    .Build().Value,
                Company.Builder()
                    .SetName("Praca zdalna Pratyki i staże")
                    .Build().Value,
                Company.Builder()
                    .SetName("Praca zdalna Witaj Świecie")
                    .Build().Value,
                Company.Builder()
                    .SetName("Praca zdalna Witkac")
                    .Build().Value,
                Company.Builder()
                    .SetName("Pratyki i staże")
                    .Build().Value,
                Company.Builder()
                    .SetName("Witaj Świecie")
                    .Build().Value,
                Company.Builder()
                    .SetName("Witkac sp.zoo")
                    .Build().Value,
            };

            List<AbsenceType> absenceTypes = new List<AbsenceType>
            {
                AbsenceType.Builder()
                    .SetName("Chorobowe")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Delegacja")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Dyżur domowy")
                    .SetIcon(BlazorBootstrap.IconName.TelephoneFill)
                    .SetPendingIconColor("#FFFFFF")
                    .SetPendingBackgroundColor("#808080")
                    .SetAcceptedIconColor("#FFFFFF")
                    .SetAcceptedBackgroundColor("#0000FF")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Dzień wolny za nadgodziny")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Dzień wolny za pracę w niedzielę")
                    .Build().Value,

                AbsenceType.Builder()
                    .SetName("Dzień wolny za pracę w sobotę")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Dzień wolny za pracę w święto")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Dzień wolny za święto przypadające w sobotę")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Godziny do odrobienia")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Godziny wolne z nadgodzin")
                    .Build().Value,

                AbsenceType.Builder()
                    .SetName("Izolacja")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Krwiodawstwo")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Kwarantanna")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Nieobiecność niepłatna usprawiedliwiona")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Nieobiecność nieusprawiedliwiona")
                    .Build().Value,

                AbsenceType.Builder()
                    .SetName("Nieobiecność usprawiedliwiona płatna (całodniwa)")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Nieobiecność usprawiedliwiona płatna (godzinowa")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Opieka nad dzieckiem zdrowym do 14 lat")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Praca zdalna")
                    .SetIcon(BlazorBootstrap.IconName.HouseDoorFill)
                    .SetPendingIconColor("#FFFFFF")
                    .SetPendingBackgroundColor("#808080")
                    .SetAcceptedIconColor("#FFFFFF")
                    .SetAcceptedBackgroundColor("#0000FF")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Szkolenie")
                    .Build().Value,

                AbsenceType.Builder()
                    .SetName("Ukryte")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Urlop bezpłatny")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Urlop macierzyński")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Urlop na żądanie")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Urlop ojcowski")
                    .Build().Value,

                AbsenceType.Builder()
                    .SetName("Urlop okolicznościwy")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Urlop rodzicielski")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Urlop wypoczynkowy")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Wyjście służbowe")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Zasiłek chorobowy")
                    .Build().Value,

                AbsenceType.Builder()
                    .SetName("Zasiłek opiekuńczy - nad chorym członkiem rodziny")
                    .Build().Value,
                AbsenceType.Builder()
                    .SetName("Zasiłek rehabilitacyjny")
                    .Build().Value,
            };

            if ((await holidaysService.GetAllAsync<Holiday>()).Value.Count == 1)
            {
                var httpClient = new HttpClient();
                try
                {
                    var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    int currentYear = DateTime.Now.Year;
                    List<Holiday> holidays = new List<Holiday>();

                    for (int i = -_yearsRange; i <= _yearsRange; i++)
                    {
                        var year = currentYear + i;
                        var response =
                            await httpClient.GetAsync($"https://date.nager.at/api/v3/publicholidays/{year}/PL");

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpRequestException(
                                $"Failed to get public holidays for {year}. StatusCode: {response.StatusCode}");
                        }

                        var jsonStream = await response.Content.ReadAsStreamAsync();
                        var localPublicHolidays =
                            await JsonSerializer.DeserializeAsync<List<PublicHoliday>>(jsonStream, jsonSerializerOptions)
                            ?? throw new InvalidOperationException($"Failed to deserialize public holidays for {year}.");

                        foreach (var holidayResult in localPublicHolidays.Select(publicHoliday => Holiday.Builder()
                                     .SetName(publicHoliday.Name)
                                     .SetLocalName(publicHoliday.LocalName)
                                     .SetStartDate(DateOnly.FromDateTime(publicHoliday.Date))
                                     .SetEndDate(DateOnly.FromDateTime(publicHoliday.Date))
                                     .Build()))
                        {
                            if (holidayResult.IsFailure)
                            {
                                throw new InvalidOperationException(holidayResult.Error);
                            }

                            holidays.Add(holidayResult.Value);
                        }
                    }

                    var result = await holidaysService.CreateAsync<Holiday>(holidays.ToArray());
                    if (result.IsFailure)
                    {
                        throw new Exception(result.Error);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    httpClient.Dispose();
                }
            }


            foreach (Roles role in Enum.GetValues(typeof(Roles)))
            {
                if (!await roleManager.RoleExistsAsync(role.ToString()))
                {
                    RoleEntity roleRole = new RoleEntity();
                    roleRole.Name = role.ToString();
                    await roleManager.CreateAsync(roleRole);
                }
            }

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var user = new UserEntity();
                user.Email = adminEmail;
                user.EmailConfirmed = true;
                user.UserName = adminEmail;
                await userManager.CreateAsync(user, pass);
                await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }

            if (await userManager.FindByEmailAsync(memberEmail) == null)
            {
                var user = new UserEntity();
                user.Email = memberEmail;
                user.EmailConfirmed = true;
                user.UserName = memberEmail;
                await userManager.CreateAsync(user, pass);
                await userManager.AddToRoleAsync(user, Roles.Member.ToString());
            }


            if ((await companiesService.GetAllAsync<Company>()).Value.Count == 0)
            {
                foreach (var company in companyList)
                {
                    var savingCompanies = await companiesService.CreateAsync<Company>(company);
                    if (savingCompanies.IsFailure)
                    {
                        throw new Exception(savingCompanies.Error);
                    }
                }

            }

            if ((await absenceTypesService.GetAllAsync<AbsenceType>()).Value.Count == 0)
            {
                foreach (AbsenceType absenceType in absenceTypes)
                {
                    var result = await absenceTypesService.CreateAsync<AbsenceType>(absenceType);
                    if (result.IsFailure)
                    {
                        throw new Exception(result.Error);
                    }
                }
            }

            await (await contextFactory.CreateDbContextAsync()).SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

public class PublicHoliday
{
    public DateTime Date { get; set; }
    public string LocalName { get; set; }
    public string Name { get; set; }
    public string CountryCode { get; set; }
    public bool Fixed { get; set; }
    public bool Global { get; set; }
    public string[] Counties { get; set; }
    public int? LaunchYear { get; set; }
    public string[] Types { get; set; }
}
