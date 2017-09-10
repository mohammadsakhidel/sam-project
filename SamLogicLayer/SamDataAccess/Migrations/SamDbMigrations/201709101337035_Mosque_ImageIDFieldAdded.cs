namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mosque_ImageIDFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("core.Mosques", "ImageID", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("core.Mosques", "ImageID");
        }
    }
}
