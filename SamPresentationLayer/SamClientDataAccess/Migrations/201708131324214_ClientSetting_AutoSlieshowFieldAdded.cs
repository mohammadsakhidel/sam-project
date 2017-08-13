namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientSetting_AutoSlieshowFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientSettings", "AutoSlideShow", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientSettings", "AutoSlideShow");
        }
    }
}
