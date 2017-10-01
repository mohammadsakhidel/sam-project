namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsolationImageEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConsolationImages",
                c => new
                    {
                        ConsolationID = c.Int(nullable: false),
                        Bytes = c.Binary(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        LastUpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ConsolationID);
            
            AddColumn("dbo.DownloadImageTasks", "Type", c => c.String(nullable: false, maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DownloadImageTasks", "Type");
            DropTable("dbo.ConsolationImages");
        }
    }
}
