namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedUrl : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Films", "RecommendLevel", c => c.Int(nullable: false));
            DropColumn("dbo.Films", "SourceUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Films", "SourceUrl", c => c.String());
            AlterColumn("dbo.Films", "RecommendLevel", c => c.Byte(nullable: false));
        }
    }
}
