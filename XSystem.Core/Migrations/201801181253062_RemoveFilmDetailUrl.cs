namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFilmDetailUrl : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Films", "DetailUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Films", "DetailUrl", c => c.String());
        }
    }
}
