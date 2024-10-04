using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackerBlazorWitkac.Interfaces.Repository;

namespace TimeTrackerBlazorWitkac.Data.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class UserEntity : IdentityUser, IDbKey<string>
    {
        public DateOnly? DateOfBirth { get; set; }
        public DateOnly? LastModify {  get; set; }
        public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
        public virtual ICollection<UserCardEntity> UserCards { get; set; }
        public virtual ICollection<UserCardEntity> DeletedUserCards { get; set; }
        public virtual ICollection<AttendanceEntity> ResolvedAttendances { get; set; }
        public virtual ICollection<AttendanceEntity> Attendances { get; set; }
        public virtual ICollection<AbsenceEntity> Absences { get; set; }
        public virtual ICollection<ConfirmationTokenEntity> ConfirmationTokens { get; set; }
    }

    public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            
        }
    }
}
