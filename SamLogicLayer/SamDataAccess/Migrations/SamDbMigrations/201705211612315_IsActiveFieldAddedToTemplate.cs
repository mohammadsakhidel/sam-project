namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsActiveFieldAddedToTemplate : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Templates", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("core.Templates", "IsActive");
        }
    }
}
