using System.Data.Entity.ModelConfiguration;

namespace Ligi.Infrastructure.Sql.AspnetSimpleMembership
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            this.Property(p => p.Id).HasColumnOrder(0);

            this.Property(p => p.UserName)
                .IsRequired().HasMaxLength(200);

            this.Property(p => p.FirstName)
                .IsOptional().HasMaxLength(100);

            this.Property(p => p.LastName)
                .IsOptional().HasMaxLength(100);

            this.HasMany(a => a.Roles).WithMany(b => b.Users).Map(m =>
                {
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                    m.ToTable("webpages_UsersInRoles");
                });
        }
    }
}
