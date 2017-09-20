namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageForeignKeyRemovedFromMosques : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Mosques", "ImageID", "dbo.Blobs");
            DropIndex("dbo.Mosques", new[] { "ImageID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Mosques", "ImageID");
            AddForeignKey("dbo.Mosques", "ImageID", "dbo.Blobs", "ID");
        }
    }
}
