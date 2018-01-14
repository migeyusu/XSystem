namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "LastUpdateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Actors", "LastUpdateTime");
        }
    }
}
