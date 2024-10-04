using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackerBlazorWitkac.Data.Entities.BaseEntities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Data.Entities;

// Represents the database entity for recording absences.
public class AbsenceEntity : DateTimeBaseEntity<int>
{
    public string UserId { get; set; }
    public int AbsenceTypeId { get; set; }
    public bool IsFullDate { get; set; }
    public ConfirmationStatus StatusOfType { get; set; }
    public ConfirmationStatus StatusOfDates { get; set; }
    public ConfirmationStatus IsFullyConfirmed { get; set; }
    public string? Reason { get; set; }
    public new DateOnly EndDate { get; set; }
    public UserEntity User { get; set; }
    public AbsenceTypeEntity AbsenceType { get; set; }
}

// Configuration class for mapping the AbsenceEntity to the database schema.
public class AbsenceEntityConfiguration : IEntityTypeConfiguration<AbsenceEntity>
{
    public void Configure(EntityTypeBuilder<AbsenceEntity> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired(true);

        builder.Property(c => c.AbsenceTypeId)
            .IsRequired(true);
        
        builder.Property(c => c.StatusOfType)
            .IsRequired(true);
        
        builder.Property(c => c.StatusOfDates)
            .IsRequired(true);

        builder.Property(c => c.StartDate)
            .IsRequired(true);
        
        builder.Property(c => c.StartHour)
            .IsRequired(true);
        
        builder.Property(c => c.StartMinute)
            .IsRequired(true);

        builder.Property(c => c.EndDate)
            .IsRequired(true);
        
        builder.Property(c => c.EndHour)
            .IsRequired(true);
        
        builder.Property(c => c.EndMinute)
            .IsRequired(true);
        
        builder.Property(c => c.IsFullDate)
            .IsRequired(true);

        builder.Property(c => c.Reason)
            .HasMaxLength(Absence.MaxReasonLength)
            .IsRequired(false);

        builder.HasOne(uc => uc.User)
            .WithMany(u => u.Absences)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(uc => uc.AbsenceType)
            .WithMany(u => u.Absences)
            .HasForeignKey(uc => uc.AbsenceTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}