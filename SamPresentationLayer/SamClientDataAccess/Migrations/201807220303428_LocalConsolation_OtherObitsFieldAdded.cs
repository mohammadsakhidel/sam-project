namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocalConsolation_OtherObitsFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LocalConsolations", "OtherObits", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocalConsolations", "OtherObits");
        }
    }
}
