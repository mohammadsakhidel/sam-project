namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeSyncingFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blobs", "LastUpdateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blobs", "LastUpdateTime");
        }
    }
}
