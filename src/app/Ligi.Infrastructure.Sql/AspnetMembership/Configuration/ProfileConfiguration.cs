using System.Data.Entity.ModelConfiguration;

namespace Ligi.Infrastructure.Sql.AspnetMembership.Configuration
{
    public class ProfileConfiguration : EntityTypeConfiguration<Profile>
    {
        public ProfileConfiguration()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.PropertyNames)
                .IsRequired();

            this.Property(t => t.PropertyValueStrings)
                .IsRequired();

            this.Property(t => t.PropertyValueBinary)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Profiles");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.PropertyNames).HasColumnName("PropertyNames");
            this.Property(t => t.PropertyValueStrings).HasColumnName("PropertyValueStrings");
            this.Property(t => t.PropertyValueBinary).HasColumnName("PropertyValueBinary");
            this.Property(t => t.LastUpdatedDate).HasColumnName("LastUpdatedDate");

            // Relationships
            this.HasRequired(t => t.User)
                .WithOptional(t => t.Profile);
        }
    }
}
