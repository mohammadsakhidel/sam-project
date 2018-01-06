namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consolation_FieldsChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consolations", "ExtraData", c => c.String(maxLength: 512));
            AlterColumn("dbo.Consolations", "TemplateInfo", c => c.String(maxLength: 1024));
            AlterColumn("dbo.Consolations", "Audience", c => c.String(maxLength: 512));
            AlterColumn("dbo.Consolations", "From", c => c.String(maxLength: 512));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Consolations", "From", c => c.String(maxLength: 64));
            AlterColumn("dbo.Consolations", "Audience", c => c.String(maxLength: 64));
            AlterColumn("dbo.Consolations", "TemplateInfo", c => c.String(maxLength: 256));
            DropColumn("dbo.Consolations", "ExtraData");
        }
    }
}
