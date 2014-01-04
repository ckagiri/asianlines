using System.Data.Entity.Migrations;

namespace Ligi.Infrastructure.Sql.Migrations
{
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
