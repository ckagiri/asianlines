namespace AsianLines.Infrastructure.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagsToTeam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "Tags", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Teams", "Tags");
        }
    }
}
