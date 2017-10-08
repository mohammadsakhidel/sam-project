namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Payment_TypefieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "Type", c => c.String(nullable: false, maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "Type");
        }
    }
}
