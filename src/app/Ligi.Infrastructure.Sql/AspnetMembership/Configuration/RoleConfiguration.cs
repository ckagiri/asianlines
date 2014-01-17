using System.Data.Entity.ModelConfiguration;

namespace Ligi.Infrastructure.Sql.AspnetMembership.Configuration
{
    public class RoleConfiguration : EntityTypeConfiguration<Role>
    {
        public RoleConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.RoleId);

            // Properties
            this.Property(t => t.RoleName)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.Description)
                .HasMaxLength(256);

            // Table & Column Mappings
            this.ToTable("Roles");
            this.Property(t => t.RoleId).HasColumnName("RoleId");
            this.Property(t => t.ApplicationId).HasColumnName("ApplicationId");
            this.Property(t => t.RoleName).HasColumnName("RoleName");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            this.HasMany(t => t.Users)
                .WithMany(t => t.Roles)
                .Map(m =>
                    {
                        m.ToTable("UsersInRoles");
                        m.MapLeftKey("RoleId");
                        m.MapRightKey("UserId");
                    });

            this.HasRequired(t => t.Application)
                .WithMany(t => t.Roles)
                .HasForeignKey(d => d.ApplicationId);
        }
    }
}
