namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class USER_FieldsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false, maxLength: 32));
            AddColumn("dbo.AspNetUsers", "Surname", c => c.String(nullable: false, maxLength: 32));
            AddColumn("dbo.AspNetUsers", "Gender", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "BirthYear", c => c.Int());
            AddColumn("dbo.AspNetUsers", "AccessLevel", c => c.String(maxLength: 1024));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AccessLevel");
            DropColumn("dbo.AspNetUsers", "BirthYear");
            DropColumn("dbo.AspNetUsers", "Gender");
            DropColumn("dbo.AspNetUsers", "Surname");
            DropColumn("dbo.AspNetUsers", "FirstName");
        }
    }
}
