namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BannerAdded : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "blob.Blobs", newSchema: "dbo");
            MoveTable(name: "core.Cities", newSchema: "dbo");
            MoveTable(name: "core.Provinces", newSchema: "dbo");
            MoveTable(name: "core.Consolations", newSchema: "dbo");
            MoveTable(name: "core.Customers", newSchema: "dbo");
            MoveTable(name: "core.Obits", newSchema: "dbo");
            MoveTable(name: "core.Mosques", newSchema: "dbo");
            MoveTable(name: "core.Saloons", newSchema: "dbo");
            MoveTable(name: "core.ObitHoldings", newSchema: "dbo");
            MoveTable(name: "core.Templates", newSchema: "dbo");
            MoveTable(name: "core.TemplateCategories", newSchema: "dbo");
            MoveTable(name: "core.TemplateFields", newSchema: "dbo");
            MoveTable(name: "core.Displays", newSchema: "dbo");
            CreateTable(
                "dbo.Banners",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 64),
                        ImageID = c.String(nullable: false, maxLength: 32),
                        LifeBeginTime = c.DateTime(),
                        LifeEndTime = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                        ShowOnStart = c.Boolean(nullable: false),
                        DurationSeconds = c.Int(nullable: false),
                        Interval = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        Creator = c.String(nullable: false, maxLength: 16),
                        LastUpdateTime = c.DateTime(nullable: false),
                        CityID = c.Int(),
                        ProvinceID = c.Int(),
                        ObitHoldingID = c.Int(),
                        MosqueID = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Banners");
            MoveTable(name: "dbo.Displays", newSchema: "core");
            MoveTable(name: "dbo.TemplateFields", newSchema: "core");
            MoveTable(name: "dbo.TemplateCategories", newSchema: "core");
            MoveTable(name: "dbo.Templates", newSchema: "core");
            MoveTable(name: "dbo.ObitHoldings", newSchema: "core");
            MoveTable(name: "dbo.Saloons", newSchema: "core");
            MoveTable(name: "dbo.Mosques", newSchema: "core");
            MoveTable(name: "dbo.Obits", newSchema: "core");
            MoveTable(name: "dbo.Customers", newSchema: "core");
            MoveTable(name: "dbo.Consolations", newSchema: "core");
            MoveTable(name: "dbo.Provinces", newSchema: "core");
            MoveTable(name: "dbo.Cities", newSchema: "core");
            MoveTable(name: "dbo.Blobs", newSchema: "blob");
        }
    }
}
