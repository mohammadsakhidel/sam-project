namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Template_CategoryFieldExcluded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories");
            DropIndex("dbo.Templates", new[] { "TemplateCategoryID" });
            DropTable("dbo.TemplateCategories");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TemplateCategories",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                        Order = c.Int(nullable: false),
                        Description = c.String(maxLength: 256),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.Templates", "TemplateCategoryID");
            AddForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories", "ID", cascadeDelete: true);
        }
    }
}
