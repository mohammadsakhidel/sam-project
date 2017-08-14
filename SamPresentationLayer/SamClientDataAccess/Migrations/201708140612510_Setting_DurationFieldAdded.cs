namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Setting_DurationFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientSettings", "DefaultSlideDurationMilliSeconds", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientSettings", "DefaultSlideDurationMilliSeconds");
        }
    }
}
