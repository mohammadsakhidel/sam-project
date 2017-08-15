namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DisplaySyncFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientSettings", "LastDisplaysUploadTime", c => c.DateTime());
            AddColumn("dbo.Displays", "SyncStatus", c => c.String(maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Displays", "SyncStatus");
            DropColumn("dbo.ClientSettings", "LastDisplaysUploadTime");
        }
    }
}
