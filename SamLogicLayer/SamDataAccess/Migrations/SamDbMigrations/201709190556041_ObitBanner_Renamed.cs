namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ObitBanner_Renamed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Banners", "ObitID", c => c.Int());
            DropColumn("dbo.Banners", "ObitHoldingID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Banners", "ObitHoldingID", c => c.Int());
            DropColumn("dbo.Banners", "ObitID");
        }
    }
}
