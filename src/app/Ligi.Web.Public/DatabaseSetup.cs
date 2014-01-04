using System.Data.Entity;
using Ligi.Infrastructure.Sql.Database;
using Ligi.Infrastructure.Sql.Migrations;

namespace Ligi.Web.Public
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