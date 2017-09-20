namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BannerSupportAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consolations", "AmountToPay", c => c.Double(nullable: false));
            AddColumn("dbo.Obits", "DeceasedIdentifier", c => c.String(maxLength: 16));
            AddColumn("dbo.Mosques", "ImageID", c => c.String(maxLength: 32));
            AddColumn("dbo.TemplateCategories", "Visible", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TemplateCategories", "Visible");
            DropColumn("dbo.Mosques", "ImageID");
            DropColumn("dbo.Obits", "DeceasedIdentifier");
            DropColumn("dbo.Consolations", "AmountToPay");
        }
    }
}
