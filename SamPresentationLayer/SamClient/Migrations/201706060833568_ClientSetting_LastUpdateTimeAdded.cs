namespace SamClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientSetting_LastUpdateTimeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientSettings", "LastUpdateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientSettings", "LastUpdateTime");
        }
    }
}
