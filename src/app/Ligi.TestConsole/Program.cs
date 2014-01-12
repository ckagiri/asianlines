using System;
using System.Data.Entity;
using Ligi.Infrastructure.Sql.Database;
using Ligi.Infrastructure.Sql.Migrations;

namespace Ligi.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing Database...");

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AdminDbContext, Configuration>());
            CreateDatabase();

            Console.WriteLine("Done...");
            Console.ReadLine();
        }

        private static void CreateDatabase()
        {
            var context = new AdminDbContext();
            context.Database.Initialize(true);
        }
    }
}
