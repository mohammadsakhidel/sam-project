namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Banner_ImageIDForeignKeyAdded : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Banners", "ImageID");
            AddForeignKey("dbo.Banners", "ImageID", "dbo.Blobs", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Banners", "ImageID", "dbo.Blobs");
            DropIndex("dbo.Banners", new[] { "ImageID" });
        }
    }
}
