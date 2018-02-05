namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitAfterMigratedToLocalEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocalBanners",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 64),
                        ImageBytes = c.Binary(),
                        LifeBeginTime = c.DateTime(),
                        LifeEndTime = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                        ShowOnStart = c.Boolean(nullable: false),
                        DurationSeconds = c.Int(nullable: false),
                        Interval = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        LastUpdateTime = c.DateTime(nullable: false),
                        Type = c.String(nullable: false, maxLength: 16),
                        ObitID = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ClientSettings",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        CityID = c.Int(nullable: false),
                        MosqueID = c.Int(nullable: false),
                        MosqueName = c.String(maxLength: 128),
                        MosqueAddress = c.String(maxLength: 512),
                        SaloonID = c.String(nullable: false, maxLength: 16),
                        DownloadIntervalMilliSeconds = c.Int(nullable: false),
                        DownloadDelayMilliSeconds = c.Int(nullable: false),
                        AutoSlideShow = c.Boolean(nullable: false),
                        DefaultSlideDurationMilliSeconds = c.Int(nullable: false),
                        LastUpdateTime = c.DateTime(),
                        LastDisplaysUploadTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.LocalConsolations",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        ObitID = c.Int(nullable: false),
                        Status = c.String(nullable: false, maxLength: 16),
                        PaymentStatus = c.String(nullable: false, maxLength: 16),
                        CreationTime = c.DateTime(nullable: false),
                        LastUpdateTime = c.DateTime(),
                        TrackingNumber = c.String(nullable: false, maxLength: 16),
                        ExtraData = c.String(maxLength: 512),
                        ImageBytes = c.Binary(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.LocalObits", t => t.ObitID, cascadeDelete: true)
                .Index(t => t.ObitID);
            
            CreateTable(
                "dbo.LocalObits",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 32),
                        MosqueID = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        LastUpdateTime = c.DateTime(),
                        TrackingNumber = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.LocalObitHoldings",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        ObitID = c.Int(nullable: false),
                        BeginTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        SaloonID = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.LocalObits", t => t.ObitID, cascadeDelete: true)
                .Index(t => t.ObitID);
            
            CreateTable(
                "dbo.LocalDisplays",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ConsolationID = c.Int(nullable: false),
                        TimeOfDisplay = c.DateTime(nullable: false),
                        DurationMilliSeconds = c.Int(nullable: false),
                        SyncStatus = c.String(maxLength: 16),
                        CreationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DownloadImageTasks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AssociatedObjectID = c.Int(nullable: false),
                        DownloadData = c.String(maxLength: 128),
                        Status = c.String(nullable: false, maxLength: 16),
                        Type = c.String(nullable: false, maxLength: 16),
                        CreationTime = c.DateTime(nullable: false),
                        DownloadCompletiontime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalConsolations", "ObitID", "dbo.LocalObits");
            DropForeignKey("dbo.LocalObitHoldings", "ObitID", "dbo.LocalObits");
            DropIndex("dbo.LocalObitHoldings", new[] { "ObitID" });
            DropIndex("dbo.LocalConsolations", new[] { "ObitID" });
            DropTable("dbo.DownloadImageTasks");
            DropTable("dbo.LocalDisplays");
            DropTable("dbo.LocalObitHoldings");
            DropTable("dbo.LocalObits");
            DropTable("dbo.LocalConsolations");
            DropTable("dbo.ClientSettings");
            DropTable("dbo.LocalBanners");
        }
    }
}
