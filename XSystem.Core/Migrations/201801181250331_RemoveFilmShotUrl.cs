namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFilmShotUrl : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Films", "ImageUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Films", "ImageUrl", c => c.String());
        }
    }
}
