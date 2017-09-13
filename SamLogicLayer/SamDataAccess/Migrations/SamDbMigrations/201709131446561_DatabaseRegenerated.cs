namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatabaseRegenerated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "blob.Blobs",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 32),
                        Bytes = c.Binary(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        LastUpdateTime = c.DateTime(),
                        ThumbImageBytes = c.Binary(),
                        ImageFormat = c.String(maxLength: 8),
                        ImageWidth = c.Int(),
                        ImageHeight = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "core.Cities",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        ProvinceID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Provinces", t => t.ProvinceID, cascadeDelete: true)
                .Index(t => t.ProvinceID);
            
            CreateTable(
                "core.Provinces",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "core.Consolations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ObitID = c.Int(nullable: false),
                        CustomerID = c.Int(nullable: false),
                        TemplateID = c.Int(nullable: false),
                        TemplateInfo = c.String(maxLength: 256),
                        Audience = c.String(maxLength: 64),
                        From = c.String(maxLength: 64),
                        Status = c.String(nullable: false, maxLength: 16),
                        PaymentStatus = c.String(nullable: false, maxLength: 16),
                        CreationTime = c.DateTime(nullable: false),
                        LastUpdateTime = c.DateTime(),
                        TrackingNumber = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Customers", t => t.CustomerID, cascadeDelete: true)
                .ForeignKey("core.Obits", t => t.ObitID, cascadeDelete: true)
                .ForeignKey("core.Templates", t => t.TemplateID, cascadeDelete: true)
                .Index(t => t.ObitID)
                .Index(t => t.CustomerID)
                .Index(t => t.TemplateID);
            
            CreateTable(
                "core.Customers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 64),
                        Gender = c.Boolean(),
                        IsMember = c.Boolean(nullable: false),
                        UserName = c.String(maxLength: 32),
                        RegistrationTime = c.DateTime(),
                        CellPhoneNumber = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "core.Obits",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 32),
                        ObitType = c.String(nullable: false, maxLength: 16),
                        MosqueID = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        LastUpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Mosques", t => t.MosqueID, cascadeDelete: true)
                .Index(t => t.MosqueID);
            
            CreateTable(
                "core.Mosques",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
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
                        ImageID = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("blob.Blobs", t => t.ImageID)
                .Index(t => t.ImageID);
            
            CreateTable(
                "core.Saloons",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 16),
                        MosqueID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                        EndpointIP = c.String(maxLength: 16),
                    })
                .PrimaryKey(t => new { t.ID, t.MosqueID })
                .ForeignKey("core.Mosques", t => t.MosqueID, cascadeDelete: true)
                .Index(t => t.MosqueID);
            
            CreateTable(
                "core.ObitHoldings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ObitID = c.Int(nullable: false),
                        BeginTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        SaloonID = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Obits", t => t.ObitID, cascadeDelete: true)
                .Index(t => t.ObitID);
            
            CreateTable(
                "core.Templates",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.ID)
                .ForeignKey("blob.Blobs", t => t.BackgroundImageID, cascadeDelete: true)
                .ForeignKey("core.TemplateCategories", t => t.TemplateCategoryID, cascadeDelete: true)
                .Index(t => t.TemplateCategoryID)
                .Index(t => t.BackgroundImageID);
            
            CreateTable(
                "core.TemplateCategories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                        Order = c.Int(nullable: false),
                        Description = c.String(maxLength: 256),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "core.TemplateFields",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
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
                .ForeignKey("core.Templates", t => t.TemplateID, cascadeDelete: true)
                .Index(t => t.TemplateID);
            
            CreateTable(
                "core.Displays",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ConsolationID = c.Int(nullable: false),
                        TimeOfDisplay = c.DateTime(nullable: false),
                        DurationMilliSeconds = c.Int(nullable: false),
                        SyncStatus = c.String(maxLength: 16),
                        CreationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Consolations", t => t.ConsolationID, cascadeDelete: true)
                .Index(t => t.ConsolationID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Type = c.String(maxLength: 16),
                        DisplayName = c.String(maxLength: 32),
                        AccessLevel = c.String(),
                        CreationTime = c.DateTime(),
                        Creator = c.String(maxLength: 32),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 32),
                        Surname = c.String(nullable: false, maxLength: 32),
                        Gender = c.Boolean(),
                        BirthYear = c.Int(),
                        IsApproved = c.Boolean(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        Creator = c.String(nullable: false, maxLength: 32),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("core.Displays", "ConsolationID", "core.Consolations");
            DropForeignKey("core.Consolations", "TemplateID", "core.Templates");
            DropForeignKey("core.TemplateFields", "TemplateID", "core.Templates");
            DropForeignKey("core.Templates", "TemplateCategoryID", "core.TemplateCategories");
            DropForeignKey("core.Templates", "BackgroundImageID", "blob.Blobs");
            DropForeignKey("core.ObitHoldings", "ObitID", "core.Obits");
            DropForeignKey("core.Saloons", "MosqueID", "core.Mosques");
            DropForeignKey("core.Obits", "MosqueID", "core.Mosques");
            DropForeignKey("core.Mosques", "ImageID", "blob.Blobs");
            DropForeignKey("core.Consolations", "ObitID", "core.Obits");
            DropForeignKey("core.Consolations", "CustomerID", "core.Customers");
            DropForeignKey("core.Cities", "ProvinceID", "core.Provinces");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("core.Displays", new[] { "ConsolationID" });
            DropIndex("core.TemplateFields", new[] { "TemplateID" });
            DropIndex("core.Templates", new[] { "BackgroundImageID" });
            DropIndex("core.Templates", new[] { "TemplateCategoryID" });
            DropIndex("core.ObitHoldings", new[] { "ObitID" });
            DropIndex("core.Saloons", new[] { "MosqueID" });
            DropIndex("core.Mosques", new[] { "ImageID" });
            DropIndex("core.Obits", new[] { "MosqueID" });
            DropIndex("core.Consolations", new[] { "TemplateID" });
            DropIndex("core.Consolations", new[] { "CustomerID" });
            DropIndex("core.Consolations", new[] { "ObitID" });
            DropIndex("core.Cities", new[] { "ProvinceID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("core.Displays");
            DropTable("core.TemplateFields");
            DropTable("core.TemplateCategories");
            DropTable("core.Templates");
            DropTable("core.ObitHoldings");
            DropTable("core.Saloons");
            DropTable("core.Mosques");
            DropTable("core.Obits");
            DropTable("core.Customers");
            DropTable("core.Consolations");
            DropTable("core.Provinces");
            DropTable("core.Cities");
            DropTable("blob.Blobs");
        }
    }
}
