using AsianLines.Core.Model;

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
        {
            context.Teams.AddOrUpdate(
                p => p.Code,
                new Team
                    {
                        Id = Guid.NewGuid(),
                        Name = "Arsenal",
                        Code = "ARS",
                        HomeGround = "Emirates Stadium"
                    },
                new Team
                    {
                        Id = Guid.NewGuid(),
                        Name = "Chelsea",
                        Code = "CHE",
                        HomeGround = "Stamford Bridge"
                    }
                );
        }
    }
}
