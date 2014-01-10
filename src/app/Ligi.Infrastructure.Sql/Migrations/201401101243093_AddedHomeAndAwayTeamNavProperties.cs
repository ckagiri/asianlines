namespace Ligi.Infrastructure.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHomeAndAwayTeamNavProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Fixtures", "Team_Id", c => c.Guid());
            CreateIndex("dbo.Fixtures", "Team_Id");
            CreateIndex("dbo.Fixtures", "AwayTeamId");
            CreateIndex("dbo.Fixtures", "HomeTeamId");
            AddForeignKey("dbo.Fixtures", "Team_Id", "dbo.Teams", "Id");
            AddForeignKey("dbo.Fixtures", "AwayTeamId", "dbo.Teams", "Id");
            AddForeignKey("dbo.Fixtures", "HomeTeamId", "dbo.Teams", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Fixtures", "HomeTeamId", "dbo.Teams");
            DropForeignKey("dbo.Fixtures", "AwayTeamId", "dbo.Teams");
            DropForeignKey("dbo.Fixtures", "Team_Id", "dbo.Teams");
            DropIndex("dbo.Fixtures", new[] { "HomeTeamId" });
            DropIndex("dbo.Fixtures", new[] { "AwayTeamId" });
            DropIndex("dbo.Fixtures", new[] { "Team_Id" });
            DropColumn("dbo.Fixtures", "Team_Id");
        }
    }
}
