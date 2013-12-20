using System.Data.Entity;
using AsianLines.Infrastructure.Sql.Database;
using AsianLines.Infrastructure.Sql.Migrations;

namespace AsianLines.Web.Admin
{
    internal static class DatabaseSetup
    {
        public static void Initialize()
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