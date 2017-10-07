namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Payment_LastUpdateAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "LastUpdateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "LastUpdateTime");
        }
    }
}
