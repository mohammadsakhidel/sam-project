namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Role_AcessLengthUnlimited : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetRoles", "AccessLevel", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetRoles", "AccessLevel", c => c.String(maxLength: 1024));
        }
    }
}
