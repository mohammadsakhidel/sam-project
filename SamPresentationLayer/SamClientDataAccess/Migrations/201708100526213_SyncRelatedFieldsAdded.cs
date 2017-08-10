namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncRelatedFieldsAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories");
            DropPrimaryKey("dbo.TemplateCategories");
            AddColumn("dbo.Consolations", "LastUpdateTime", c => c.DateTime());
            AddColumn("dbo.Obits", "LastUpdateTime", c => c.DateTime());
            AddColumn("dbo.Templates", "LastUpdateTime", c => c.DateTime());
            AlterColumn("dbo.TemplateCategories", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TemplateCategories", "ID");
            AddForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories");
            DropPrimaryKey("dbo.TemplateCategories");
            AlterColumn("dbo.TemplateCategories", "ID", c => c.Int(nullable: false));
            DropColumn("dbo.Templates", "LastUpdateTime");
            DropColumn("dbo.Obits", "LastUpdateTime");
            DropColumn("dbo.Consolations", "LastUpdateTime");
            AddPrimaryKey("dbo.TemplateCategories", "ID");
            AddForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories", "ID", cascadeDelete: true);
        }
    }
}
