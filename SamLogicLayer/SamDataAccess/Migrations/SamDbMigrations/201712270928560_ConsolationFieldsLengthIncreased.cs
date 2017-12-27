namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsolationFieldsLengthIncreased : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Consolations", "Audience", c => c.String(maxLength: 512));
            AlterColumn("dbo.Consolations", "From", c => c.String(maxLength: 512));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Consolations", "From", c => c.String(maxLength: 64));
            AlterColumn("dbo.Consolations", "Audience", c => c.String(maxLength: 64));
        }
    }
}
