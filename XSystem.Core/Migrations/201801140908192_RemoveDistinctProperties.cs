namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDistinctProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "Region", c => c.Int(nullable: false));
            AddColumn("dbo.Films", "Region", c => c.Int(nullable: false));
            AddColumn("dbo.Publishers", "Region", c => c.Int(nullable: false));
            DropColumn("dbo.Actors", "FilmSearchUrl");
            DropColumn("dbo.Actors", "FetchRegion");
            DropColumn("dbo.Films", "FetchRegion");
            DropColumn("dbo.Publishers", "FetchRegion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Publishers", "FetchRegion", c => c.String());
            AddColumn("dbo.Films", "FetchRegion", c => c.String());
            AddColumn("dbo.Actors", "FetchRegion", c => c.String());
            AddColumn("dbo.Actors", "FilmSearchUrl", c => c.String());
            DropColumn("dbo.Publishers", "Region");
            DropColumn("dbo.Films", "Region");
            DropColumn("dbo.Actors", "Region");
        }
    }
}
