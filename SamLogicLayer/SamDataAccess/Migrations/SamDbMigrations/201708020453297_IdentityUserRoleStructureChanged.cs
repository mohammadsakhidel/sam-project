namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdentityUserRoleStructureChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "Type", c => c.String(maxLength: 16));
            AddColumn("dbo.AspNetRoles", "DisplayName", c => c.String(maxLength: 32));
            AddColumn("dbo.AspNetRoles", "AccessLevel", c => c.String(maxLength: 1024));
            DropColumn("dbo.AspNetUsers", "AccessLevel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "AccessLevel", c => c.String(maxLength: 1024));
            DropColumn("dbo.AspNetRoles", "AccessLevel");
            DropColumn("dbo.AspNetRoles", "DisplayName");
            DropColumn("dbo.AspNetRoles", "Type");
        }
    }
}
