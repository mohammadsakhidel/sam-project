namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Role_CreatorAndCreationTimeFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "CreationTime", c => c.DateTime());
            AddColumn("dbo.AspNetRoles", "Creator", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "Creator");
            DropColumn("dbo.AspNetRoles", "CreationTime");
        }
    }
}
