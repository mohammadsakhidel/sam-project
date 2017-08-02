namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AspNetUser_CreattorFieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CreationTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "Creator", c => c.String(nullable: false, maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Creator");
            DropColumn("dbo.AspNetUsers", "CreationTime");
        }
    }
}
