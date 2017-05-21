namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NameFieldAddedToTemplate : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Templates", "Name", c => c.String(nullable: false, maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("core.Templates", "Name");
        }
    }
}
