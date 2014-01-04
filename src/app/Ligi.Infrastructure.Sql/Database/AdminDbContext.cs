using System.Data.Entity;
using Ligi.Core.Model;

namespace Ligi.Infrastructure.Sql.Database
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext() : base(nameOrConnectionString: "AsianLines")
        { }

        public AdminDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Season>().HasMany(s => s.Teams).WithMany();
        }

        public DbSet<League> Leagues { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Fixture> Fixtures { get; set; }
    }
}
