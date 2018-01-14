namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewProperties1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "IsInitialAccomplish", c => c.Boolean(nullable: false));
            AddColumn("dbo.Actors", "LastUpdateDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Films", "DetailUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Films", "DetailUrl");
            DropColumn("dbo.Actors", "LastUpdateDateTime");
            DropColumn("dbo.Actors", "IsInitialAccomplish");
        }
    }
}
