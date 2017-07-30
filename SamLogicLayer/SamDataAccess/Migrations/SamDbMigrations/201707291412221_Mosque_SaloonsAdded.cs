namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mosque_SaloonsAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Saloons",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MosqueID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                        EndpointIP = c.String(maxLength: 16),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Mosques", t => t.MosqueID, cascadeDelete: true)
                .Index(t => t.MosqueID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Saloons", "MosqueID", "core.Mosques");
            DropIndex("dbo.Saloons", new[] { "MosqueID" });
            DropTable("dbo.Saloons");
        }
    }
}
