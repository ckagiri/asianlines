using System.Data.Entity.ModelConfiguration;

namespace Ligi.Infrastructure.Sql.AspnetMembership.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Users");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.ApplicationId).HasColumnName("ApplicationId");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.IsAnonymous).HasColumnName("IsAnonymous");
            this.Property(t => t.LastActivityDate).HasColumnName("LastActivityDate");

            // Relationships
            this.HasRequired(t => t.Application)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.ApplicationId);
        }
    }
}
