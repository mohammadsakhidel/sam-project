namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateRemovedFromClientDatabase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TemplateFields", "TemplateID", "dbo.Templates");
            DropForeignKey("dbo.Consolations", "TemplateID", "dbo.Templates");
            DropIndex("dbo.Consolations", new[] { "TemplateID" });
            DropIndex("dbo.TemplateFields", new[] { "TemplateID" });
            DropTable("dbo.Templates");
            DropTable("dbo.TemplateFields");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TemplateFields",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        TemplateID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 16),
                        DisplayName = c.String(nullable: false, maxLength: 32),
                        Description = c.String(maxLength: 128),
                        X = c.Double(nullable: false),
                        Y = c.Double(nullable: false),
                        FontFamily = c.String(maxLength: 64),
                        FontSize = c.String(maxLength: 10),
                        Bold = c.Boolean(),
                        TextColor = c.String(maxLength: 10),
                        FlowDirection = c.String(maxLength: 4),
                        BoxWidth = c.Double(nullable: false),
                        BoxHeight = c.Double(nullable: false),
                        HorizontalContentAlignment = c.String(maxLength: 16),
                        VerticalContentAlignment = c.String(maxLength: 16),
                        WrapContent = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Templates",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                        Order = c.Int(nullable: false),
                        TemplateCategoryID = c.Int(nullable: false),
                        BackgroundImageID = c.String(nullable: false, maxLength: 32),
                        Text = c.String(nullable: false, maxLength: 1024),
                        Price = c.Double(nullable: false),
                        WidthRatio = c.Int(nullable: false),
                        HeightRatio = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        Creator = c.String(nullable: false, maxLength: 32),
                        LastUpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.TemplateFields", "TemplateID");
            CreateIndex("dbo.Consolations", "TemplateID");
            AddForeignKey("dbo.Consolations", "TemplateID", "dbo.Templates", "ID", cascadeDelete: true);
            AddForeignKey("dbo.TemplateFields", "TemplateID", "dbo.Templates", "ID", cascadeDelete: true);
        }
    }
}
