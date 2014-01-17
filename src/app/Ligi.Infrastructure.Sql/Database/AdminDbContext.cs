using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Ligi.Core.DataAccess;
using Ligi.Core.Model;
using Ligi.Infrastructure.Sql.AspnetMembership;
using Ligi.Infrastructure.Sql.AspnetMembership.Configuration;

namespace Ligi.Infrastructure.Sql.Database
{
    public class AdminDbContext : DbContext, IContext
    {
        public AdminDbContext() : base(nameOrConnectionString: ConnectionStringName)
        { }

        public AdminDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        { }

        public DbSet<League> Leagues { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Fixture> Fixtures { get; set; }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UsersOpenAuthAccount> UsersOpenAuthAccounts { get; set; }
        public DbSet<UsersOpenAuthData> UsersOpenAuthData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Season>().HasMany(s => s.Teams).WithMany();

            modelBuilder.Entity<Fixture>()
                .HasRequired(f => f.HomeTeam)
                .WithMany()
                .HasForeignKey(f => f.HomeTeamId);

            modelBuilder.Entity<Fixture>()
                .HasRequired(f => f.AwayTeam)
                .WithMany()
                .HasForeignKey(f => f.AwayTeamId);

            // Aspnet Membership
            modelBuilder.Configurations.Add(new ApplicationConfiguration());
            modelBuilder.Configurations.Add(new MembershipConfiguration());
            modelBuilder.Configurations.Add(new ProfileConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new UsersOpenAuthAccountConfiguration());
            modelBuilder.Configurations.Add(new UsersOpenAuthDataConfiguration());

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public static string ConnectionStringName
        {
            get
            {
                if (ConfigurationManager.AppSettings["ConnectionStringName"]
                    != null)
                {
                    return ConfigurationManager.
                        AppSettings["ConnectionStringName"];
                }

                return "DefaultConnection";
            }
        }

        public T Find<T>(Guid id) where T : class
        {
            return Set<T>().Find(id);
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return Set<T>();
        }
    }
}
