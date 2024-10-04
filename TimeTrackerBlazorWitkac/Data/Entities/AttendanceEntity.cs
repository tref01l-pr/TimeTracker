using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackerBlazorWitkac.Data.Entities.BaseEntities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Data.Entities;

// The AttendanceEntity class represents the database entity for attendance.
public class AttendanceEntity : DateTimeBaseEntity<int>
{
    public int UserCardId { get; set; }
    public string UserId { get; set; }
    public int CompanyId { get; set; }
    public bool IsStrangeActivity { get; set; }
    public string? StrangeActivityReason { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedById { get; set; }
    
    public UserCardEntity UserCard { get; set; }
    public UserEntity User { get; set; }
    public CompanyEntity Company { get; set; }
    public UserEntity ResolvedBy { get; set; }
}

// Configuration class for mapping the AttendanceEntity to the database schema.
public class AttendanceEntityConfiguration : IEntityTypeConfiguration<AttendanceEntity>
{
    public void Configure(EntityTypeBuilder<AttendanceEntity> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserCardId)
            .IsRequired(true);
        
        builder.Property(a => a.UserId)
            .IsRequired(true);
        
        builder.Property(a => a.CompanyId)
            .IsRequired(true);
        
        builder.Property(a => a.StartDate)
            .IsRequired(true);
        
        builder.Property(a => a.StartHour)
            .IsRequired(true);
        
        builder.Property(a => a.StartMinute)
            .IsRequired(true);
        
        builder.Property(a => a.EndDate)
            .IsRequired(false);
        
        builder.Property(a => a.EndHour)
            .IsRequired(true);
        
        builder.Property(a => a.EndMinute)
            .IsRequired(true);

        builder.Property(a => a.IsStrangeActivity)
            .IsRequired(true);
        
        builder.Property(a => a.StrangeActivityReason)
            .HasMaxLength(Attendance.MaxStrangeActivityReasonLength)
            .IsRequired(false);
        
        builder.Property(a => a.ResolvedAt)
            .IsRequired(false);
        
        builder.Property(a => a.ResolvedById)
            .IsRequired(false);
        
        builder.HasOne(a => a.UserCard)
            .WithMany(uc => uc.Attendances)
            .HasForeignKey(a => a.UserCardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(uc => uc.User)
            .WithMany(u => u.Attendances)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(uc => uc.ResolvedBy)
            .WithMany(u => u.ResolvedAttendances)
            .HasForeignKey(uc => uc.ResolvedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(uc => uc.Company)
            .WithMany(u => u.Attendances)
            .HasForeignKey(uc => uc.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
