namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSeperationFetchProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "FilmSearchUrl", c => c.String());
            AddColumn("dbo.Actors", "FetchRegion", c => c.String());
            DropColumn("dbo.Actors", "SeedSearchUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Actors", "SeedSearchUrl", c => c.String());
            DropColumn("dbo.Actors", "FetchRegion");
            DropColumn("dbo.Actors", "FilmSearchUrl");
        }
    }
}
