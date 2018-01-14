namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReptilehistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReptileHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FetchErrorPages",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Url = c.String(),
                        Exception = c.String(),
                        ReptileHistory_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReptileHistories", t => t.ReptileHistory_Id,cascadeDelete:true)
                .Index(t => t.ReptileHistory_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FetchErrorPages", "ReptileHistory_Id", "dbo.ReptileHistories");
            DropIndex("dbo.FetchErrorPages", new[] { "ReptileHistory_Id" });
            DropTable("dbo.FetchErrorPages");
            DropTable("dbo.ReptileHistories");
        }
    }
}
