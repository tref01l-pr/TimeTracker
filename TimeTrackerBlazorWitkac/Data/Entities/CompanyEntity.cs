using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackerBlazorWitkac.Data.Entities.BaseEntities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Data.Entities;

// The CompanyEntity class represents the database entity for companies.
public class CompanyEntity : BaseEntity<int>
{
    public string Name { get; set; }
    public DateOnly? DateOfFoundation { get; set; }
    
    public virtual ICollection<AttendanceEntity> Attendances { get; set; }
    public virtual ICollection<UserCardEntity> UserCards { get; set; }
}

// Configuration class for mapping the CompanyEntity to the database schema.
public class CompanyEntityConfiguration : IEntityTypeConfiguration<CompanyEntity>
{
    public void Configure(EntityTypeBuilder<CompanyEntity> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(Company.MaxNameLength)
            .IsRequired(true);
    }
}