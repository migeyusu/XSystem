namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtraProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Films", "FileLocation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Films", "FileLocation");
        }
    }
}
