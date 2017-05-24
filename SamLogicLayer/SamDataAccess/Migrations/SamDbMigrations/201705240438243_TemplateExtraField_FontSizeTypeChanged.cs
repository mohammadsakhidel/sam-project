namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateExtraField_FontSizeTypeChanged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("core.TemplateExtraFields", "FontSize", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("core.TemplateExtraFields", "FontSize", c => c.Double(nullable: false));
        }
    }
}
