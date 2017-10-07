namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Payment_Created : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 32),
                        Token = c.String(nullable: false, maxLength: 1024),
                        Amount = c.Int(nullable: false),
                        Provider = c.String(nullable: false, maxLength: 32),
                        Status = c.String(nullable: false, maxLength: 16),
                        ReferenceCode = c.String(maxLength: 64),
                        CreationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Payments");
        }
    }
}
