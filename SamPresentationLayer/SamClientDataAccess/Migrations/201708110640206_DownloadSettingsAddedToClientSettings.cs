namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DownloadSettingsAddedToClientSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientSettings", "DownloadIntervalMilliSeconds", c => c.Int(nullable: false));
            AddColumn("dbo.ClientSettings", "DownloadDelayMilliSeconds", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientSettings", "DownloadDelayMilliSeconds");
            DropColumn("dbo.ClientSettings", "DownloadIntervalMilliSeconds");
        }
    }
}
