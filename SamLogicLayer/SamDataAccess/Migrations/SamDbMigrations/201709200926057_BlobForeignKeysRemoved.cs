namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BlobForeignKeysRemoved : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Banners", "ImageID", "dbo.Blobs");
            DropForeignKey("dbo.Templates", "BackgroundImageID", "dbo.Blobs");
            DropIndex("dbo.Banners", new[] { "ImageID" });
            DropIndex("dbo.Templates", new[] { "BackgroundImageID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Templates", "BackgroundImageID");
            CreateIndex("dbo.Banners", "ImageID");
            AddForeignKey("dbo.Templates", "BackgroundImageID", "dbo.Blobs", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Banners", "ImageID", "dbo.Blobs", "ID", cascadeDelete: true);
        }
    }
}
