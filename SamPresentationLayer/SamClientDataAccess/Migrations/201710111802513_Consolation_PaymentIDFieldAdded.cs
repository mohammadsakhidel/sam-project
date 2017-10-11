namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consolation_PaymentIDFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consolations", "PaymentID", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Consolations", "PaymentID");
        }
    }
}
