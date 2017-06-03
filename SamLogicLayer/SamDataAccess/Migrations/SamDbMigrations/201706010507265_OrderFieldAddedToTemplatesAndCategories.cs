namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderFieldAddedToTemplatesAndCategories : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Templates", "Order", c => c.Int(nullable: false));
            AddColumn("core.TemplateCategories", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("core.TemplateCategories", "Order");
            DropColumn("core.Templates", "Order");
        }
    }
}
