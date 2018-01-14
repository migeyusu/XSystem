namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReviseActorUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "SeedSearchUrl", c => c.String());
            AddColumn("dbo.Actors", "ParentUrl", c => c.String());
            DropColumn("dbo.Actors", "DmmSearchUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Actors", "DmmSearchUrl", c => c.String());
            DropColumn("dbo.Actors", "ParentUrl");
            DropColumn("dbo.Actors", "SeedSearchUrl");
        }
    }
}
