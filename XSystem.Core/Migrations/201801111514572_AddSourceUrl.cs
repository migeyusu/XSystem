namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSourceUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "SourceUrl", c => c.String());
            AddColumn("dbo.Films", "SourceUrl", c => c.String());
            DropColumn("dbo.Actors", "ParentUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Actors", "ParentUrl", c => c.String());
            DropColumn("dbo.Films", "SourceUrl");
            DropColumn("dbo.Actors", "SourceUrl");
        }
    }
}
