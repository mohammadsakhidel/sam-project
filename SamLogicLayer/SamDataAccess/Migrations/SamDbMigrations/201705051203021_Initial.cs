namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "blob.Blobs",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 32),
                        Name = c.String(nullable: false, maxLength: 64),
                        Extension = c.String(nullable: false, maxLength: 8),
                        Bytes = c.Binary(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
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
                        Audience = c.String(nullable: false, maxLength: 64),
                        From = c.String(nullable: false, maxLength: 64),
                        Status = c.String(nullable: false, maxLength: 16),
                        PaymentStatus = c.String(nullable: false, maxLength: 16),
                        CreationTime = c.DateTime(nullable: false),
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
                        Name = c.String(nullable: false, maxLength: 32),
                        Surname = c.String(nullable: false, maxLength: 32),
                        Gender = c.Boolean(),
                        IsMember = c.Boolean(nullable: false),
                        UserName = c.String(maxLength: 32),
                        RegistrationTime = c.DateTime(),
                        CellPhoneNumber = c.String(maxLength: 16),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "core.Obits",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MosqueID = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Mosques", t => t.MosqueID, cascadeDelete: true)
                .Index(t => t.MosqueID);
            
            CreateTable(
                "core.DeceasedPersons",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ObitID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                        Surname = c.String(nullable: false, maxLength: 32),
                        Gender = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Obits", t => t.ObitID, cascadeDelete: true)
                .Index(t => t.ObitID);
            
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
                        Creator = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Cities", t => t.CityID, cascadeDelete: true)
                .Index(t => t.CityID);
            
            CreateTable(
                "core.ObitHoldings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ObitID = c.Int(nullable: false),
                        BeginTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Obits", t => t.ObitID, cascadeDelete: true)
                .Index(t => t.ObitID);
            
            CreateTable(
                "core.Templates",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TemplateCategoryID = c.Int(nullable: false),
                        BackgroundImageID = c.String(nullable: false, maxLength: 32),
                        Text = c.String(nullable: false, maxLength: 1024),
                        Price = c.Double(nullable: false),
                        WidthRatio = c.Int(nullable: false),
                        HeightRatio = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        Creator = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.TemplateCategories", t => t.TemplateCategoryID, cascadeDelete: true)
                .Index(t => t.TemplateCategoryID);
            
            CreateTable(
                "core.TemplateCategories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                        Description = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "core.TemplateExtraFields",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TemplateID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 16),
                        DisplayName = c.String(nullable: false, maxLength: 32),
                        X = c.Int(nullable: false),
                        Y = c.Int(nullable: false),
                        FontFamily = c.String(maxLength: 64),
                        FontSize = c.Double(nullable: false),
                        Bold = c.Boolean(),
                        FlowDirection = c.String(maxLength: 4),
                        BoxWidth = c.Int(nullable: false),
                        BoxHeight = c.Int(nullable: false),
                        HorizontalContentAlignment = c.String(maxLength: 16),
                        VerticalContentAlignment = c.String(maxLength: 16),
                        WrapContent = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Templates", t => t.TemplateID, cascadeDelete: true)
                .Index(t => t.TemplateID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
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
                        IsApproved = c.Boolean(nullable: false),
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
            DropForeignKey("core.Consolations", "TemplateID", "core.Templates");
            DropForeignKey("core.TemplateExtraFields", "TemplateID", "core.Templates");
            DropForeignKey("core.Templates", "TemplateCategoryID", "core.TemplateCategories");
            DropForeignKey("core.ObitHoldings", "ObitID", "core.Obits");
            DropForeignKey("core.Obits", "MosqueID", "core.Mosques");
            DropForeignKey("core.Mosques", "CityID", "core.Cities");
            DropForeignKey("core.DeceasedPersons", "ObitID", "core.Obits");
            DropForeignKey("core.Consolations", "ObitID", "core.Obits");
            DropForeignKey("core.Consolations", "CustomerID", "core.Customers");
            DropForeignKey("core.Cities", "ProvinceID", "core.Provinces");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("core.TemplateExtraFields", new[] { "TemplateID" });
            DropIndex("core.Templates", new[] { "TemplateCategoryID" });
            DropIndex("core.ObitHoldings", new[] { "ObitID" });
            DropIndex("core.Mosques", new[] { "CityID" });
            DropIndex("core.DeceasedPersons", new[] { "ObitID" });
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
            DropTable("core.TemplateExtraFields");
            DropTable("core.TemplateCategories");
            DropTable("core.Templates");
            DropTable("core.ObitHoldings");
            DropTable("core.Mosques");
            DropTable("core.DeceasedPersons");
            DropTable("core.Obits");
            DropTable("core.Customers");
            DropTable("core.Consolations");
            DropTable("core.Provinces");
            DropTable("core.Cities");
            DropTable("blob.Blobs");
        }
    }
}
