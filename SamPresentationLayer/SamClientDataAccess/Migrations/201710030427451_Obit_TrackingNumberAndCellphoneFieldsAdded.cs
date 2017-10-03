namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Obit_TrackingNumberAndCellphoneFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Obits", "OwnerCellPhone", c => c.String(nullable: false, maxLength: 16));
            AddColumn("dbo.Obits", "TrackingNumber", c => c.String(nullable: false, maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Obits", "TrackingNumber");
            DropColumn("dbo.Obits", "OwnerCellPhone");
        }
    }
}
