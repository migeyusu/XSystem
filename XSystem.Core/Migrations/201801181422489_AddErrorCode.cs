namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddErrorCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FetchErrorPages", "ErrorCode", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FetchErrorPages", "ErrorCode");
        }
    }
}
