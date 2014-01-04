using System.Data.Entity.Migrations;

namespace Ligi.Infrastructure.Sql.Migrations
{
    public partial class Add_League_Season_Fixture_Tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Fixtures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SeasonId = c.Guid(nullable: false),
                        HomeTeamId = c.Guid(nullable: false),
                        AwayTeamId = c.Guid(nullable: false),
                        KickOff = c.DateTime(nullable: false),
                        Venue = c.String(),
                        MatchStatus = c.Int(nullable: false),
                        HomeScore = c.Int(nullable: false),
                        AwayScore = c.Int(nullable: false),
                        HomeAsianHandicap = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AwayAsianHandicap = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AsianGoalsHandicap = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StartOfWeek = c.DateTime(nullable: false),
                        EndOfWeek = c.DateTime(nullable: false),
                        MatchResultConfirmed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MatchOdds",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FixtureId = c.Guid(nullable: false),
                        OddsType = c.Int(nullable: false),
                        Handicap = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Fixtures", t => t.FixtureId, cascadeDelete: true)
                .Index(t => t.FixtureId);
            
            CreateTable(
                "dbo.Leagues",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Seasons",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        LeagueId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Leagues", t => t.LeagueId, cascadeDelete: true)
                .Index(t => t.LeagueId);
            
            CreateTable(
                "dbo.SeasonTeams",
                c => new
                    {
                        Season_Id = c.Guid(nullable: false),
                        Team_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Season_Id, t.Team_Id })
                .ForeignKey("dbo.Seasons", t => t.Season_Id, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.Team_Id, cascadeDelete: true)
                .Index(t => t.Season_Id)
                .Index(t => t.Team_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SeasonTeams", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.SeasonTeams", "Season_Id", "dbo.Seasons");
            DropForeignKey("dbo.Seasons", "LeagueId", "dbo.Leagues");
            DropForeignKey("dbo.MatchOdds", "FixtureId", "dbo.Fixtures");
            DropIndex("dbo.SeasonTeams", new[] { "Team_Id" });
            DropIndex("dbo.SeasonTeams", new[] { "Season_Id" });
            DropIndex("dbo.Seasons", new[] { "LeagueId" });
            DropIndex("dbo.MatchOdds", new[] { "FixtureId" });
            DropTable("dbo.SeasonTeams");
            DropTable("dbo.Seasons");
            DropTable("dbo.Leagues");
            DropTable("dbo.MatchOdds");
            DropTable("dbo.Fixtures");
        }
    }
}
