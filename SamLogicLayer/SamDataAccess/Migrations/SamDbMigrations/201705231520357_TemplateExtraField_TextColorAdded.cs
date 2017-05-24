namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateExtraField_TextColorAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.TemplateExtraFields", "TextColor", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("core.TemplateExtraFields", "TextColor");
        }
    }
}
