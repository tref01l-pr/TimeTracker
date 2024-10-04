using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TimeTrackerBlazorWitkac.Data.Entities;

namespace TimeTrackerBlazorWitkac.Data;

/// <summary>
/// Database context for the Time Tracker application, providing access to the application's data models.
/// </summary>
/// <summary>
/// Initializes a new instance of the <see cref="TimeTrackerDbContext"/> class.
/// </summary>
/// <param name="options">The options for the database context.</param>
/// <param name="configuration">The configuration used to retrieve connection strings.</param>
public class TimeTrackerDbContext(
    DbContextOptions<TimeTrackerDbContext> options,
    IConfiguration configuration) : IdentityDbContext<UserEntity, RoleEntity, string, IdentityUserClaim<string>,
    UserRoleEntity, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>(options)
{
    public DbSet<UserCardEntity> UserCards { get; set; }
    public DbSet<HolidayEntity> Holidays { get; set; }
    public DbSet<CompanyEntity> Companies { get; set; }
    public DbSet<AttendanceEntity> Attendances { get; set; }
    public DbSet<AbsenceEntity> Absences { get; set; }
    public DbSet<AbsenceTypeEntity> AbsenceTypes { get; set; }
    public DbSet<ConfirmationTokenEntity> ConfirmationTokens { get; set; }

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation, with a transaction as the result.</returns>
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Configures the model creating process to apply configurations from the assembly.
    /// </summary>
    /// <param name="builder">The model builder used to configure the model.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(TimeTrackerDbContext).Assembly);
        base.OnModelCreating(builder);

        builder.Entity<UserRoleEntity>(entity =>
        {
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });
    }

    /// <summary>
    /// Configures the database context options.
    /// </summary>
    /// <param name="optionsBuilder">The options builder used to configure the context.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' not found.");
        optionsBuilder
            .UseSqlServer(connectionString, options => options.CommandTimeout(300));

        base.OnConfiguring(optionsBuilder);
    }
}