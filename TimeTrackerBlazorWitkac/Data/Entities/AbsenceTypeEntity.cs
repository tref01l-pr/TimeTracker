using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackerBlazorWitkac.Data.Entities.BaseEntities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Data.Entities;

// The AbsenceTypeEntity class represents the database entity for absence types.
public class AbsenceTypeEntity : BaseEntity<int>
{
    public string Name { get; set; }
    public BlazorBootstrap.IconName? Icon { get; set; }
    public string PendingIconColor { get; set; }
    public string PendingBackgroundColor { get; set; }
    public string AcceptedIconColor { get; set; }
    public string AcceptedBackgroundColor { get; set; }
    public string? Description { get; set; }
    
    public ICollection<AbsenceEntity> Absences { get; set; }
}

// Configuration class for mapping the AbsenceTypeEntity to the database schema.
public class AbsenceTypeEntityConfiguration : IEntityTypeConfiguration<AbsenceTypeEntity>
{
    public void Configure(EntityTypeBuilder<AbsenceTypeEntity> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .HasMaxLength(AbsenceType.MaxNameLength)
            .IsRequired(true);
        
        builder.Property(c => c.Icon)
            .IsRequired(false);
        
        builder.Property(c => c.PendingIconColor)
            .HasMaxLength(AbsenceType.MaxColorLength)
            .IsRequired(true);
        
        builder.Property(c => c.PendingBackgroundColor)
            .HasMaxLength(AbsenceType.MaxColorLength)
            .IsRequired(true);
        
        builder.Property(c => c.AcceptedIconColor)
            .HasMaxLength(AbsenceType.MaxColorLength)
            .IsRequired(true);
        
        builder.Property(c => c.AcceptedBackgroundColor)
            .HasMaxLength(AbsenceType.MaxColorLength)
            .IsRequired(true);

        builder.Property(c => c.Description)
            .HasMaxLength(AbsenceType.MaxDescriptionLength)
            .IsRequired(false);
    }
}