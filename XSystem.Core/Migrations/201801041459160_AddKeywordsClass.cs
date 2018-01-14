namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKeywordsClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Keywords",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Film_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Film_Id);
            AddColumn("dbo.Actors", "ShotTag", c => c.String());
            AddColumn("dbo.Films", "ShotTag", c => c.String());
            AddColumn("dbo.Films", "ImageUrl", c => c.String());
            AddColumn("dbo.Films", "Code", c => c.String());
            DropColumn("dbo.Films", "Characteristic");
            CreateTable("dbo.KeywordsFilms",
                    (builder => new {
                        Film_Id = builder.Guid(nullable: false),
                        Keywords_Id = builder.Guid(nullable: false),
                    }))
                .PrimaryKey((arg => new {arg.Film_Id, arg.Keywords_Id}))
                .ForeignKey("dbo.Films", (arg => arg.Film_Id), cascadeDelete: true)
                .ForeignKey("dbo.Keywords", (arg => arg.Keywords_Id), cascadeDelete: true)
                .Index((arg => arg.Film_Id))
                .Index((arg => arg.Keywords_Id));
        }
        
        public override void Down()
        {
            AddColumn("dbo.Films", "Characteristic", c => c.String());
            DropForeignKey("dbo.KeywordsFilms", "Film_Id", "dbo.Films");
            DropForeignKey("dbo.KeywordsFilms", "Keywords_Id", "dbo.Keywords");
            DropIndex("dbo.KeywordsFilms", new[] { "Film_Id" });
            DropIndex("dbo.KeywordsFilms", new[] { "Keywords_Id" });
            DropColumn("dbo.Films", "Code");
            DropColumn("dbo.Films", "ImageUrl");
            DropColumn("dbo.Films", "ShotTag");
            DropColumn("dbo.Actors", "ShotTag");
            DropTable("dbo.Keywords");
            DropTable("dbo.KeywordsFilms");
        }
    }
}
