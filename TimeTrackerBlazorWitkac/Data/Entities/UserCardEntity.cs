using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackerBlazorWitkac.Data.Entities.BaseEntities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Data.Entities;

// Represents the database entity for user cards.
public class UserCardEntity : BaseEntity<int>
{
    public string UserId { get; set; }
    public int CompanyId { get; set; }
    public string? UserDeletedId { get; set; } 
    public string Number { get; set; }
    public string Name { get; set; }
    public CardType CardType { get; set; }
    public bool IsActive { get; set; }
    public DateOnly CreatedAt { get; set; }
    public DateOnly? DeletedAt { get; set; }
    
    public virtual UserEntity User { get; set; }
    public virtual UserEntity UserDeleted { get; set; }
    public virtual CompanyEntity Company { get; set; }
    public virtual ICollection<AttendanceEntity> Attendances { get; set; }
}

// Configuration class for UserCardEntity defining its mapping in the database.
public class UserCardEntityConfiguration : IEntityTypeConfiguration<UserCardEntity>
{
    public void Configure(EntityTypeBuilder<UserCardEntity> builder)
    {
        builder.HasKey(uc => uc.Id);

        builder.Property(uc => uc.UserId)
            .IsRequired(true);

        builder.Property(uc => uc.CompanyId)
            .IsRequired(true);

        builder.Property(uc => uc.Number)
            .IsRequired(true);

        builder.Property(uc => uc.Name)
            .HasMaxLength(UserCard.MaxNameLength)
            .IsRequired(false);

        builder.Property(uc => uc.CardType)
            .IsRequired(true);

        builder.Property(uc => uc.IsActive)
            .IsRequired(true);

        builder.Property(uc => uc.CreatedAt)
            .IsRequired(true);

        builder.HasOne(uc => uc.User)
            .WithMany(u => u.UserCards)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(uc => uc.UserDeleted)
            .WithMany(u => u.DeletedUserCards)
            .HasForeignKey(uc => uc.UserDeletedId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(uc => uc.Company)
            .WithMany(u => u.UserCards)
            .HasForeignKey(uc => uc.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
