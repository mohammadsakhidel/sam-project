namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Blob_NameFieldRemoved : DbMigration
    {
        public override void Up()
        {
            DropColumn("blob.Blobs", "Name");
        }
        
        public override void Down()
        {
            AddColumn("blob.Blobs", "Name", c => c.String(nullable: false, maxLength: 64));
        }
    }
}
