using Microsoft.AspNetCore.Identity;
using System.Globalization;
using TimeTrackerBlazorWitkac;
using TimeTrackerBlazorWitkac.Components;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Options;
using TimeTrackerBlazorWitkac.Service.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection(SmtpOptions.Smtp));

#region ForTests
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

builder.Services.AddRepositories();
builder.Services.AddScopedAndTransientServices();
builder.Services.AddAutoMapperProfiles();


builder.Services.AddHttpContextAccessor();

builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddSingleton<IEmailSender<UserEntity>, EmailSenderService>();

builder.Services.AddRazorAndBlazorServices();

var app = builder.Build();


    await SeedData(app);

async Task SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory?.CreateScope())
    {
        var service = scope?.ServiceProvider.GetService<Seed>();
        await service?.SeedDataContextAsync()!;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    #region ForTests

    app.UseSwagger();
    app.UseSwaggerUI();

    #endregion
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

#region ForTests

app.MapControllers();

#endregion
var culture = new CultureInfo("pl-PL");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();
app.Run();
