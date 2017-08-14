namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consolation_TrackingNumberFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Consolations", "TrackingNumber", c => c.String(nullable: false, maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("core.Consolations", "TrackingNumber");
        }
    }
}
