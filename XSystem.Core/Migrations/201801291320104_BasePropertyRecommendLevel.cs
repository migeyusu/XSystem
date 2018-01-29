namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BasePropertyRecommendLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "RecommendLevel", c => c.Int(nullable: false));
            AddColumn("dbo.Series", "RecommendLevel", c => c.Int(nullable: false));
            AddColumn("dbo.Publishers", "RecommendLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Publishers", "RecommendLevel");
            DropColumn("dbo.Series", "RecommendLevel");
            DropColumn("dbo.Actors", "RecommendLevel");
        }
    }
}
