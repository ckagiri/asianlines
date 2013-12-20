using System.Data.Entity;
using AsianLines.Core.Model;

namespace AsianLines.Infrastructure.Sql.Database
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext() : base(nameOrConnectionString: "AsianLines")
        { }

        public AdminDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        { }

        public DbSet<Team> Teams { get; set; }
    }
}
