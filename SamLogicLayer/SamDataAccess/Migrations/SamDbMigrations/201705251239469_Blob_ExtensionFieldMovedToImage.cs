namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Blob_ExtensionFieldMovedToImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("blob.Blobs", "ImageFormat", c => c.String(maxLength: 8));
            DropColumn("blob.Blobs", "Extension");
        }
        
        public override void Down()
        {
            AddColumn("blob.Blobs", "Extension", c => c.String(nullable: false, maxLength: 8));
            DropColumn("blob.Blobs", "ImageFormat");
        }
    }
}
