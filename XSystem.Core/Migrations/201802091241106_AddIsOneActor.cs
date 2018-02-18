namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsOneActor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Films", "IsOneActor", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Films", "IsOneActor");
        }
    }
}
