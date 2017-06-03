namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateField_DescPropAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.TemplateFields", "Description", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("core.TemplateFields", "Description");
        }
    }
}
