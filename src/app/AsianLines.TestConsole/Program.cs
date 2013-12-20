using AsianLines.Infrastructure.Sql.Database;

namespace AsianLines.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateDatabase();
        }

        private static void CreateDatabase()
        {
            var context = new AdminDbContext();
            context.Database.Initialize(true);
        }
    }
}
