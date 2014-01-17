using System.Data.Entity.ModelConfiguration;

namespace Ligi.Infrastructure.Sql.AspnetMembership.Configuration
{
    public class UsersOpenAuthAccountConfiguration : EntityTypeConfiguration<UsersOpenAuthAccount>
    {
        public UsersOpenAuthAccountConfiguration()
        {
            // Primary Key
            this.HasKey(t => new { t.ApplicationName, t.ProviderName, t.ProviderUserId });

            // Properties
            this.Property(t => t.ApplicationName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.ProviderName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.ProviderUserId)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.ProviderUserName)
                .IsRequired();

            this.Property(t => t.MembershipUserName)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("UsersOpenAuthAccounts");
            this.Property(t => t.ApplicationName).HasColumnName("ApplicationName");
            this.Property(t => t.ProviderName).HasColumnName("ProviderName");
            this.Property(t => t.ProviderUserId).HasColumnName("ProviderUserId");
            this.Property(t => t.ProviderUserName).HasColumnName("ProviderUserName");
            this.Property(t => t.MembershipUserName).HasColumnName("MembershipUserName");
            this.Property(t => t.LastUsedUtc).HasColumnName("LastUsedUtc");

            // Relationships
            this.HasRequired(t => t.UsersOpenAuthData)
                .WithMany(t => t.UsersOpenAuthAccounts)
                .HasForeignKey(d => new { d.ApplicationName, d.MembershipUserName });
        }
    }
}
