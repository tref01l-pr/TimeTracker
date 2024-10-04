using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Radzen;
using TimeTrackerBlazorWitkac.Components.Account;
using TimeTrackerBlazorWitkac.Data;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Repositories;
using TimeTrackerBlazorWitkac.Interfaces;
using TimeTrackerBlazorWitkac.Interfaces.Repository;
using TimeTrackerBlazorWitkac.Interfaces.Services;
using TimeTrackerBlazorWitkac.Interfaces.Services.Application;
using TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;
using TimeTrackerBlazorWitkac.Service;
using TimeTrackerBlazorWitkac.Service.Application;

namespace TimeTrackerBlazorWitkac;

public static class ServiceRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAbsencesRepository, AbsencesRepository>();
        services.AddScoped<IAbsenceTypesRepository, AbsenceTypesRepository>();
        services.AddScoped<IAttendancesRepository, AttendancesRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IUserCardsRepository, UserCardsRepository>();
        services.AddScoped<ICompaniesRepository, CompaniesRepository>();
        services.AddScoped<IHolidaysRepository, HolidaysRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IConfirmationTokensRepository, ConfirmationTokensRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }

    public static IServiceCollection AddScopedAndTransientServices(this IServiceCollection services)
    {
        services.AddTransient<Seed>();
        services.AddScoped<IClipboardService, ClipboardService>();
        services.AddScoped<IToastNotificationService, ToastNotificationService>();
        services.AddScoped<ISchedulerService, SchedulerService>();
        services.AddScoped<ICardSwipeService, CardSwipeService>();
        services.AddScoped<IModalAbsencesService, ModalAbsencesService>();
        services.AddScoped<IDetailAbsencesService, DetailAbsencesService>();
        services.AddScoped<IAbsenceStatusService, AbsenceStatusService>();
        services.AddScoped<IClaimsTransformation, ClaimsFactory>();

        services.AddApplicationServices();
        
        return services;
    }
    
    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAbsencesService, AbsencesService>();
        services.AddScoped<IAbsenceTypesService, AbsenceTypesService>();
        services.AddScoped<IAttendancesService, AttendancesService>();
        services.AddScoped<IAttendanceChecker, AttendanceChecker>();
        services.AddScoped<IHolidaysService, HolidaysService>();
        services.AddScoped<IUserCardsService, UserCardsService>();
        services.AddScoped<ICompaniesService, CompaniesService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IRolesService, RolesService>();
        
        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<TimeTrackerDbContext>();
        services.AddDbContext<TimeTrackerDbContext>();
        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }

    public static IServiceCollection AddRazorAndBlazorServices(this IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        services.AddBlazorBootstrap();
        services.AddRadzenComponents();

        return services;
    }

    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile<AbsenceMappingProfile>();
            config.AddProfile<AbsenceTypeMappingProfile>();
            config.AddProfile<AttendanceMappingProfile>();
            config.AddProfile<CompanyMappingProfile>();
            config.AddProfile<HolidayMappingProfile>();
            config.AddProfile<UserMappingProfile>();
            config.AddProfile<UserCardMappingProfile>();
            config.AddProfile<RoleMappingProfiles>();
            config.AddProfile<ConfirmationTokenMappingProfile>();
            
            config.AddProfile<WorkDayMappingProfile>();
        });

        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdministratorOnly", policy => policy.RequireRole("Admin"));
        });

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            })
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = configuration["AzureAd:ClientId"];
                microsoftOptions.ClientSecret = configuration["AzureAd:ClientSecret"];
                microsoftOptions.AuthorizationEndpoint = configuration["AzureAd:AuthorizationEndpoint"];
                microsoftOptions.TokenEndpoint = configuration["AzureAd:TokenEndpoint"];
                
            })
            .AddIdentityCookies();

        services.AddIdentityCore<UserEntity>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<RoleEntity>()
            .AddEntityFrameworkStores<TimeTrackerDbContext>()
            .AddUserManager<UserManager<UserEntity>>()
            .AddRoleManager<RoleManager<RoleEntity>>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }
}