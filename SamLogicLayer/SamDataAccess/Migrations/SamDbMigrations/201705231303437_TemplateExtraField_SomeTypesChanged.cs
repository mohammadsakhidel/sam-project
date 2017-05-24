namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateExtraField_SomeTypesChanged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("core.TemplateExtraFields", "X", c => c.Double(nullable: false));
            AlterColumn("core.TemplateExtraFields", "Y", c => c.Double(nullable: false));
            AlterColumn("core.TemplateExtraFields", "BoxWidth", c => c.Double(nullable: false));
            AlterColumn("core.TemplateExtraFields", "BoxHeight", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("core.TemplateExtraFields", "BoxHeight", c => c.Int(nullable: false));
            AlterColumn("core.TemplateExtraFields", "BoxWidth", c => c.Int(nullable: false));
            AlterColumn("core.TemplateExtraFields", "Y", c => c.Int(nullable: false));
            AlterColumn("core.TemplateExtraFields", "X", c => c.Int(nullable: false));
        }
    }
}
