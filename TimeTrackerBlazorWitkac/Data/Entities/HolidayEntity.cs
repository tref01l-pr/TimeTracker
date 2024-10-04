using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackerBlazorWitkac.Data.Entities.BaseEntities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Data.Entities;

// The HolidayEntity class represents the database entity for holidays.
public class HolidayEntity : DateOnlyBaseEntity<int>
{
    public string Name { get; set; }
    public string LocalName { get; set; }
    public new DateOnly EndDate { get; set; }
    public string? Description { get; set; }
}

// Configuration class for mapping the HolidayEntity to the database schema.
public class HolidayEntityConfiguration : IEntityTypeConfiguration<HolidayEntity>
{
    public void Configure(EntityTypeBuilder<HolidayEntity> builder)
    {
        builder.HasKey(h => h.Id);
        
        builder.Property(uc => uc.Name)
            .HasMaxLength(Holiday.MaxNameLength)
            .IsRequired(true);
        
        builder.Property(uc => uc.LocalName)
            .HasMaxLength(Holiday.MaxNameLength)
            .IsRequired(true);
        
        builder.Property(uc => uc.StartDate)
            .IsRequired(true);
        
        builder.Property(uc => uc.EndDate)
            .IsRequired(true);

        builder.Property(uc => uc.Description)
            .HasMaxLength(Holiday.MaxDescriptionLength)
            .IsRequired(false);
    }
}