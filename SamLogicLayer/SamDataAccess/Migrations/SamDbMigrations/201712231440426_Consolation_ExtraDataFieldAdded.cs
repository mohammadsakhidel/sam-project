namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consolation_ExtraDataFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consolations", "ExtraData", c => c.String(maxLength: 512));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Consolations", "ExtraData");
        }
    }
}
