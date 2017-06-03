namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer_FieldsModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Customers", "FullName", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("core.Customers", "CellPhoneNumber", c => c.String(nullable: false, maxLength: 16));
            DropColumn("core.Customers", "Name");
            DropColumn("core.Customers", "Surname");
        }
        
        public override void Down()
        {
            AddColumn("core.Customers", "Surname", c => c.String(nullable: false, maxLength: 32));
            AddColumn("core.Customers", "Name", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("core.Customers", "CellPhoneNumber", c => c.String(maxLength: 16));
            DropColumn("core.Customers", "FullName");
        }
    }
}
