namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeSyncFieldsAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("core.Mosques", "CityID", "core.Cities");
            DropIndex("core.Mosques", new[] { "CityID" });
            AddColumn("blob.Blobs", "LastUpdateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("blob.Blobs", "LastUpdateTime");
            CreateIndex("core.Mosques", "CityID");
            AddForeignKey("core.Mosques", "CityID", "core.Cities", "ID", cascadeDelete: true);
        }
    }
}
