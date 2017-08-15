namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DisplaySyncStatusFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Displays", "SyncStatus", c => c.String(maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("core.Displays", "SyncStatus");
        }
    }
}
