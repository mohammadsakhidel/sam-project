namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedEntitiesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RemovedEntities",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EntityType = c.String(nullable: false, maxLength: 32),
                        RemovingTime = c.DateTime(nullable: false),
                        metadata = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RemovedEntities");
        }
    }
}
