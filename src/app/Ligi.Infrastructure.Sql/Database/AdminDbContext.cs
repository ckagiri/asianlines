using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Ligi.Core.DataAccess;
using Ligi.Core.Model;
using Ligi.Infrastructure.Sql.AspnetSimpleMembership;

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
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Season>().HasMany(s => s.Teams).WithMany();

            modelBuilder.Entity<Fixture>()
                .HasRequired(f => f.HomeTeam)
                .WithMany()
                .HasForeignKey(f => f.HomeTeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Fixture>()
                .HasRequired(f => f.AwayTeam)
                .WithMany()
                .HasForeignKey(f => f.AwayTeamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Configurations.Add(new UserConfiguration());

            // ASP.NET WebPages SimpleSecurity tables
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new OAuthMembershipConfiguration());
            modelBuilder.Configurations.Add(new MembershipConfiguration());
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
