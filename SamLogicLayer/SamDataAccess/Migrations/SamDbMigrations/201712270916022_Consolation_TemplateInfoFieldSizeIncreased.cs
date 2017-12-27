namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consolation_TemplateInfoFieldSizeIncreased : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Consolations", "TemplateInfo", c => c.String(maxLength: 1024));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Consolations", "TemplateInfo", c => c.String(maxLength: 256));
        }
    }
}
