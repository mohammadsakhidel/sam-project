namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Blob_ThumbBytesAddedToImages : DbMigration
    {
        public override void Up()
        {
            AddColumn("blob.Blobs", "ThumbImageBytes", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("blob.Blobs", "ThumbImageBytes");
        }
    }
}
