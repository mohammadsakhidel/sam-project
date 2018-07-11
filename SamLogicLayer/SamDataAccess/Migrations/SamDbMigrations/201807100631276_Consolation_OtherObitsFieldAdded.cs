namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consolation_OtherObitsFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consolations", "OtherObits", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Consolations", "OtherObits");
        }
    }
}
