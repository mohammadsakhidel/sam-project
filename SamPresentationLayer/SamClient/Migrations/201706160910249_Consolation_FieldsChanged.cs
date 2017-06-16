namespace SamClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Consolation_FieldsChanged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Consolations", "Audience", c => c.String(maxLength: 64));
            AlterColumn("dbo.Consolations", "From", c => c.String(maxLength: 64));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Consolations", "From", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.Consolations", "Audience", c => c.String(nullable: false, maxLength: 64));
        }
    }
}
