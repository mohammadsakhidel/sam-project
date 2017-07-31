namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ObitHolding_SaloonIDAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.ObitHoldings", "SaloonID", c => c.String(nullable: false, maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("core.ObitHoldings", "SaloonID");
        }
    }
}
