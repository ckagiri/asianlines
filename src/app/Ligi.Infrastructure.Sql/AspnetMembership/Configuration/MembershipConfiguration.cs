using System.Data.Entity.ModelConfiguration;

namespace Ligi.Infrastructure.Sql.AspnetMembership.Configuration
{
    public class MembershipConfiguration : EntityTypeConfiguration<Membership>
    {
        public MembershipConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.PasswordSalt)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Email)
                .HasMaxLength(256);

            this.Property(t => t.PasswordQuestion)
                .HasMaxLength(256);

            this.Property(t => t.PasswordAnswer)
                .HasMaxLength(128);

            this.Property(t => t.Comment)
                .HasMaxLength(256);

            // Table & Column Mappings
            this.ToTable("Memberships");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.ApplicationId).HasColumnName("ApplicationId");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.PasswordFormat).HasColumnName("PasswordFormat");
            this.Property(t => t.PasswordSalt).HasColumnName("PasswordSalt");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.PasswordQuestion).HasColumnName("PasswordQuestion");
            this.Property(t => t.PasswordAnswer).HasColumnName("PasswordAnswer");
            this.Property(t => t.IsApproved).HasColumnName("IsApproved");
            this.Property(t => t.IsLockedOut).HasColumnName("IsLockedOut");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.LastLoginDate).HasColumnName("LastLoginDate");
            this.Property(t => t.LastPasswordChangedDate).HasColumnName("LastPasswordChangedDate");
            this.Property(t => t.LastLockoutDate).HasColumnName("LastLockoutDate");
            this.Property(t => t.FailedPasswordAttemptCount).HasColumnName("FailedPasswordAttemptCount");
            this.Property(t => t.FailedPasswordAttemptWindowStart).HasColumnName("FailedPasswordAttemptWindowStart");
            this.Property(t => t.FailedPasswordAnswerAttemptCount).HasColumnName("FailedPasswordAnswerAttemptCount");
            this.Property(t => t.FailedPasswordAnswerAttemptWindowsStart).HasColumnName("FailedPasswordAnswerAttemptWindowsStart");
            this.Property(t => t.Comment).HasColumnName("Comment");

            // Relationships
            this.HasRequired(t => t.Application)
                .WithMany(t => t.Memberships)
                .HasForeignKey(d => d.ApplicationId);
            this.HasRequired(t => t.User)
                .WithOptional(t => t.Membership);
        }
    }
}
