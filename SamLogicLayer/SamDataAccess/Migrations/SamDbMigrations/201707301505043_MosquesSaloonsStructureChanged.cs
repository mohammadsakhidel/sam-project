namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MosquesSaloonsStructureChanged : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "core.Saloons",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 16),
                        MosqueID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                        EndpointIP = c.String(maxLength: 16),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("core.Mosques", t => t.MosqueID, cascadeDelete: true)
                .Index(t => t.MosqueID);
            
            AddColumn("core.Mosques", "LastUpdateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("core.Saloons", "MosqueID", "core.Mosques");
            DropIndex("core.Saloons", new[] { "MosqueID" });
            DropColumn("core.Mosques", "LastUpdateTime");
            DropTable("core.Saloons");
        }
    }
}
