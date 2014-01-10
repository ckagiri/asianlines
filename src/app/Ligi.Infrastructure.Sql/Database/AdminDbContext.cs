using System.Configuration;
using System.Data.Entity;
using Ligi.Core.Model;

namespace Ligi.Infrastructure.Sql.Database
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext() : base(nameOrConnectionString: ConnectionStringName)
        { }

        public AdminDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        { }

        public DbSet<League> Leagues { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Fixture> Fixtures { get; set; }

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
    }
}
