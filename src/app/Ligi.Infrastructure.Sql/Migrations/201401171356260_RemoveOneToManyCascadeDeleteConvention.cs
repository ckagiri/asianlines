namespace Ligi.Infrastructure.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOneToManyCascadeDeleteConvention : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MatchOdds", "FixtureId", "dbo.Fixtures");
            DropForeignKey("dbo.Seasons", "LeagueId", "dbo.Leagues");
            DropIndex("dbo.MatchOdds", new[] { "FixtureId" });
            DropIndex("dbo.Seasons", new[] { "LeagueId" });
            CreateIndex("dbo.MatchOdds", "FixtureId");
            CreateIndex("dbo.Seasons", "LeagueId");
            AddForeignKey("dbo.MatchOdds", "FixtureId", "dbo.Fixtures", "Id");
            AddForeignKey("dbo.Seasons", "LeagueId", "dbo.Leagues", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Seasons", "LeagueId", "dbo.Leagues");
            DropForeignKey("dbo.MatchOdds", "FixtureId", "dbo.Fixtures");
            DropIndex("dbo.Seasons", new[] { "LeagueId" });
            DropIndex("dbo.MatchOdds", new[] { "FixtureId" });
            CreateIndex("dbo.Seasons", "LeagueId");
            CreateIndex("dbo.MatchOdds", "FixtureId");
            AddForeignKey("dbo.Seasons", "LeagueId", "dbo.Leagues", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MatchOdds", "FixtureId", "dbo.Fixtures", "Id", cascadeDelete: true);
        }
    }
}
