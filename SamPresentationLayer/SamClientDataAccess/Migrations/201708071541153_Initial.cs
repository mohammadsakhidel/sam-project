namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blobs",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 32),
                        Bytes = c.Binary(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        ThumbImageBytes = c.Binary(),
                        ImageFormat = c.String(maxLength: 8),
                        ImageWidth = c.Int(),
                        ImageHeight = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ClientSettings",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        MosqueID = c.Int(nullable: false),
                        SaloonID = c.String(nullable: false, maxLength: 16),
                        LastUpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Consolations",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        ObitID = c.Int(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        TemplateID = c.Int(nullable: false),
                        TemplateInfo = c.String(maxLength: 256),
                        Audience = c.String(maxLength: 64),
                        From = c.String(maxLength: 64),
                        Status = c.String(nullable: false, maxLength: 16),
                        PaymentStatus = c.String(nullable: false, maxLength: 16),
                        CreationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .ForeignKey("dbo.Obits", t => t.ObitID, cascadeDelete: true)
                .ForeignKey("dbo.Templates", t => t.TemplateID, cascadeDelete: true)
                .Index(t => t.ObitID)
                .Index(t => t.CustomerID)
                .Index(t => t.TemplateID);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        FullName = c.String(nullable: false, maxLength: 64),
                        Gender = c.Boolean(),
                        IsMember = c.Boolean(nullable: false),
                        UserName = c.String(maxLength: 32),
                        RegistrationTime = c.DateTime(),
                        CellPhoneNumber = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Obits",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 32),
                        ObitType = c.String(nullable: false, maxLength: 16),
                        MosqueID = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Mosques", t => t.MosqueID, cascadeDelete: true)
                .Index(t => t.MosqueID);
            
            CreateTable(
                "dbo.Mosques",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                        ImamName = c.String(maxLength: 32),
                        ImamCellPhone = c.String(maxLength: 16),
                        InterfaceName = c.String(maxLength: 32),
                        InterfaceCellPhone = c.String(maxLength: 16),
                        CityID = c.Int(nullable: false),
                        Address = c.String(nullable: false, maxLength: 256),
                        Location = c.String(maxLength: 32),
                        PhoneNumber = c.String(nullable: false, maxLength: 16),
                        CreationTime = c.DateTime(nullable: false),
                        LastUpdateTime = c.DateTime(),
                        Creator = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Saloons",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 16),
                        MosqueID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                        EndpointIP = c.String(maxLength: 16),
                    })
                .PrimaryKey(t => new { t.ID, t.MosqueID })
                .ForeignKey("dbo.Mosques", t => t.MosqueID, cascadeDelete: true)
                .Index(t => t.MosqueID);
            
            CreateTable(
                "dbo.ObitHoldings",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        ObitID = c.Int(nullable: false),
                        BeginTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        SaloonID = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Obits", t => t.ObitID, cascadeDelete: true)
                .Index(t => t.ObitID);
            
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
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TemplateCategories", t => t.TemplateCategoryID, cascadeDelete: true)
                .Index(t => t.TemplateCategoryID);
            
            CreateTable(
                "dbo.TemplateCategories",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                        Order = c.Int(nullable: false),
                        Description = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Templates", t => t.TemplateID, cascadeDelete: true)
                .Index(t => t.TemplateID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Consolations", "TemplateID", "dbo.Templates");
            DropForeignKey("dbo.TemplateFields", "TemplateID", "dbo.Templates");
            DropForeignKey("dbo.Templates", "TemplateCategoryID", "dbo.TemplateCategories");
            DropForeignKey("dbo.ObitHoldings", "ObitID", "dbo.Obits");
            DropForeignKey("dbo.Saloons", "MosqueID", "dbo.Mosques");
            DropForeignKey("dbo.Obits", "MosqueID", "dbo.Mosques");
            DropForeignKey("dbo.Consolations", "ObitID", "dbo.Obits");
            DropForeignKey("dbo.Consolations", "CustomerID", "dbo.Customers");
            DropIndex("dbo.TemplateFields", new[] { "TemplateID" });
            DropIndex("dbo.Templates", new[] { "TemplateCategoryID" });
            DropIndex("dbo.ObitHoldings", new[] { "ObitID" });
            DropIndex("dbo.Saloons", new[] { "MosqueID" });
            DropIndex("dbo.Obits", new[] { "MosqueID" });
            DropIndex("dbo.Consolations", new[] { "TemplateID" });
            DropIndex("dbo.Consolations", new[] { "CustomerID" });
            DropIndex("dbo.Consolations", new[] { "ObitID" });
            DropTable("dbo.TemplateFields");
            DropTable("dbo.TemplateCategories");
            DropTable("dbo.Templates");
            DropTable("dbo.ObitHoldings");
            DropTable("dbo.Saloons");
            DropTable("dbo.Mosques");
            DropTable("dbo.Obits");
            DropTable("dbo.Customers");
            DropTable("dbo.Consolations");
            DropTable("dbo.ClientSettings");
            DropTable("dbo.Blobs");
        }
    }
}
