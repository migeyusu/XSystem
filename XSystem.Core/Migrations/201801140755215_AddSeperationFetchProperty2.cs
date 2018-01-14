namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSeperationFetchProperty2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Films", "FetchRegion", c => c.String());
            AddColumn("dbo.Publishers", "FetchRegion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Publishers", "FetchRegion");
            DropColumn("dbo.Films", "FetchRegion");
        }
    }
}
