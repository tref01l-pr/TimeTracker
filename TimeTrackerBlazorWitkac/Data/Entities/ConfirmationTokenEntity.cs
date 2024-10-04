using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackerBlazorWitkac.Interfaces.Repository;

namespace TimeTrackerBlazorWitkac.Data.Entities;

public class ConfirmationTokenEntity : IDbKey<int>
{
    public int Id { get; set; }
    public string UserId { get; set; } 
    public string Token { get; set; } 
    public DateTime Expiration { get; set; } 
    public ConfirmationTypes ConfirmationType { get; set; }
    
    public UserEntity User { get; set; }
}

public class ConfirmationTokenEntityConfiguration : IEntityTypeConfiguration<ConfirmationTokenEntity>
{
    public void Configure(EntityTypeBuilder<ConfirmationTokenEntity> builder)
    {
        builder.HasOne(uc => uc.User)
            .WithMany(u => u.ConfirmationTokens)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}