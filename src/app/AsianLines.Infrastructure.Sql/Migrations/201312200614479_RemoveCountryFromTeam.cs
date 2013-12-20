namespace AsianLines.Infrastructure.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCountryFromTeam : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Teams", "Country");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Teams", "Country", c => c.String());
        }
    }
}
