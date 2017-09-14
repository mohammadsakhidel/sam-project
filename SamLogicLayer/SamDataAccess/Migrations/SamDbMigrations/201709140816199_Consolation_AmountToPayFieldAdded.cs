namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consolation_AmountToPayFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Consolations", "AmountToPay", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("core.Consolations", "AmountToPay");
        }
    }
}
