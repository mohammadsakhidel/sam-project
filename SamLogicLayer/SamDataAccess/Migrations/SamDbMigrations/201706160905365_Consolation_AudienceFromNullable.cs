namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consolation_AudienceFromNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("core.Consolations", "Audience", c => c.String(maxLength: 64));
            AlterColumn("core.Consolations", "From", c => c.String(maxLength: 64));
        }
        
        public override void Down()
        {
            AlterColumn("core.Consolations", "From", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("core.Consolations", "Audience", c => c.String(nullable: false, maxLength: 64));
        }
    }
}
