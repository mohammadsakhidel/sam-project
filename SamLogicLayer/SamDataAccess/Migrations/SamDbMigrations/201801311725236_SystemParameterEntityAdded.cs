namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemParameterEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemParameters",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        LastGifCheckDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SystemParameters");
        }
    }
}
