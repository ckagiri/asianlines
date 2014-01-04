using System.Data.Entity.Migrations;

namespace Ligi.Infrastructure.Sql.Migrations
{
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
