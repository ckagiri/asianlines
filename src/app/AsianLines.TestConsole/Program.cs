using System.Data.Entity;
using AsianLines.Infrastructure.Sql.Database;
using AsianLines.Infrastructure.Sql.Migrations;

namespace AsianLines.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AdminDbContext, Configuration>());
            CreateDatabase();
        }

        private static void CreateDatabase()
        {
            var context = new AdminDbContext();
            context.Database.Initialize(true);
        }
    }
}
