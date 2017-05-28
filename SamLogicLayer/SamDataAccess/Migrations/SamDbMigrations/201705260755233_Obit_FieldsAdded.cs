namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Obit_FieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Obits", "Title", c => c.String(nullable: false, maxLength: 32));
            AddColumn("core.Obits", "ObitType", c => c.String(nullable: false, maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("core.Obits", "ObitType");
            DropColumn("core.Obits", "Title");
        }
    }
}
