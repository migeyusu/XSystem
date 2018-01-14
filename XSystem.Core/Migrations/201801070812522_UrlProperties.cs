namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UrlProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "DmmSearchUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Actors", "DmmSearchUrl");
        }
    }
}
