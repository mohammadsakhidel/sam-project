namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BannerForeignKeysToObitAndMosqueAdded : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Banners", "MosqueID");
            CreateIndex("dbo.Banners", "ObitID");
            AddForeignKey("dbo.Banners", "MosqueID", "dbo.Mosques", "ID");
            AddForeignKey("dbo.Banners", "ObitID", "dbo.Obits", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Banners", "ObitID", "dbo.Obits");
            DropForeignKey("dbo.Banners", "MosqueID", "dbo.Mosques");
            DropIndex("dbo.Banners", new[] { "ObitID" });
            DropIndex("dbo.Banners", new[] { "MosqueID" });
        }
    }
}
