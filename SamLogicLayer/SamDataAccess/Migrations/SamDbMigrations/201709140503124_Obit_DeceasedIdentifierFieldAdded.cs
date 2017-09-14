namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Obit_DeceasedIdentifierFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Obits", "DeceasedIdentifier", c => c.String(maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("core.Obits", "DeceasedIdentifier");
        }
    }
}
