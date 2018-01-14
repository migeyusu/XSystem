namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmigrationAddActorCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "Code", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Actors", "Code");
        }
    }
}
