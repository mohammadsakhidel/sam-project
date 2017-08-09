namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NeededFieldsForSyncingAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Consolations", "LastUpdateTime", c => c.DateTime());
            AddColumn("core.Obits", "LastUpdateTime", c => c.DateTime());
            AddColumn("core.Templates", "LastUpdateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("core.Templates", "LastUpdateTime");
            DropColumn("core.Obits", "LastUpdateTime");
            DropColumn("core.Consolations", "LastUpdateTime");
        }
    }
}
