namespace AsianLines.Infrastructure.Sql.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Sql.Database.AdminDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "AsianLines.Infrastructure.Sql.Database.AdminDbContext";
        }

        protected override void Seed(Sql.Database.AdminDbContext context)
        { }
    }
}
