using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Ligi.Infrastructure.Sql.AspnetSimpleMembership
{
    public class MembershipConfiguration : EntityTypeConfiguration<Membership>
    {
        public MembershipConfiguration()
        {
            this.ToTable("webpages_Membership");
            this.HasKey(p => p.UserId);

            this.Property(p => p.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(p => p.ConfirmationToken)
                .HasMaxLength(128).HasColumnType("nvarchar");

            this.Property(p => p.PasswordFailuresSinceLastSuccess)
                .IsRequired();

            this.Property(p => p.Password).IsRequired()
                .HasMaxLength(128).HasColumnType("nvarchar");

            this.Property(p => p.PasswordSalt)
                .IsRequired().HasMaxLength(128).HasColumnType("nvarchar");

            this.Property(p => p.PasswordVerificationToken)
                .HasMaxLength(128).HasColumnType("nvarchar");
        }
    }
}
