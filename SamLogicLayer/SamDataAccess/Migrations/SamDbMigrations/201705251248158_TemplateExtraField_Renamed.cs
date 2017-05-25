namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateExtraField_Renamed : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "core.TemplateExtraFields", newName: "TemplateFields");
        }
        
        public override void Down()
        {
            RenameTable(name: "core.TemplateFields", newName: "TemplateExtraFields");
        }
    }
}
