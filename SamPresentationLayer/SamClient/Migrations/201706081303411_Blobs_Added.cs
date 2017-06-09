namespace SamClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Blobs_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blobs",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 32),
                        Bytes = c.Binary(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        ThumbImageBytes = c.Binary(),
                        ImageFormat = c.String(maxLength: 8),
                        ImageWidth = c.Int(),
                        ImageHeight = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Blobs");
        }
    }
}
