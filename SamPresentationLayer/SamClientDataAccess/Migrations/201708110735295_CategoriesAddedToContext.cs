namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoriesAddedToContext : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories");
            DropPrimaryKey("dbo.TemplateCategories");
            AlterColumn("dbo.TemplateCategories", "ID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.TemplateCategories", "ID");
            AddForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories");
            DropPrimaryKey("dbo.TemplateCategories");
            AlterColumn("dbo.TemplateCategories", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TemplateCategories", "ID");
            AddForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories", "ID", cascadeDelete: true);
        }
    }
}
