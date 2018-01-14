namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Films", "LastUpdateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Actors", "LastUpdateDateTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Actors", "LastUpdateDateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Films", "LastUpdateTime");
        }
    }
}
