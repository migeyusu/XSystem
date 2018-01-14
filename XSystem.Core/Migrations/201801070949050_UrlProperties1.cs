namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UrlProperties1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "ShotUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Actors", "ShotUrl");
        }
    }
}
