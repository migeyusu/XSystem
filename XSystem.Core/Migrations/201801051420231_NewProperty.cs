namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "ShotTag", c => c.String());
            AddColumn("dbo.Films", "ShotTag", c => c.String());
            AddColumn("dbo.Films", "ImageUrl", c => c.String());
            AddColumn("dbo.Films", "RecommendLevel", c => c.Byte(nullable: false));
            AddColumn("dbo.Films", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Films", "Code");
            DropColumn("dbo.Films", "RecommendLevel");
            DropColumn("dbo.Films", "ImageUrl");
            DropColumn("dbo.Films", "ShotTag");
            DropColumn("dbo.Actors", "ShotTag");
        }
    }
}
