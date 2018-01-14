namespace XSystem.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Actors",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Films",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Characteristic = c.String(),
                        Name = c.String(),
                        Series_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Series", t => t.Series_Id,cascadeDelete:true)
                .Index(t => t.Series_Id);
            
            CreateTable(
                "dbo.Series",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Characteristic = c.String(),
                        Name = c.String(),
                        Publisher_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Publishers", t => t.Publisher_Id,cascadeDelete:true)
                .Index(t => t.Publisher_Id);
            
            CreateTable(
                "dbo.Publishers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FilmActors",
                c => new
                    {
                        Film_Id = c.Guid(nullable: false),
                        Actor_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Film_Id, t.Actor_Id })
                .ForeignKey("dbo.Films", t => t.Film_Id, cascadeDelete: true)
                .ForeignKey("dbo.Actors", t => t.Actor_Id, cascadeDelete: true)
                .Index(t => t.Film_Id)
                .Index(t => t.Actor_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Series", "Publisher_Id", "dbo.Publishers");
            DropForeignKey("dbo.Films", "Series_Id", "dbo.Series");
            DropForeignKey("dbo.FilmActors", "Actor_Id", "dbo.Actors");
            DropForeignKey("dbo.FilmActors", "Film_Id", "dbo.Films");
            DropIndex("dbo.FilmActors", new[] { "Actor_Id" });
            DropIndex("dbo.FilmActors", new[] { "Film_Id" });
            DropIndex("dbo.Series", new[] { "Publisher_Id" });
            DropIndex("dbo.Films", new[] { "Series_Id" });
            DropTable("dbo.FilmActors");
            DropTable("dbo.Publishers");
            DropTable("dbo.Series");
            DropTable("dbo.Films");
            DropTable("dbo.Actors");
        }
    }
}
