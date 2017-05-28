namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeceasedPersons_Droped : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("core.DeceasedPersons", "ObitID", "core.Obits");
            DropIndex("core.DeceasedPersons", new[] { "ObitID" });
            DropTable("core.DeceasedPersons");
        }
        
        public override void Down()
        {
            CreateTable(
                "core.DeceasedPersons",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ObitID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 32),
                        Surname = c.String(nullable: false, maxLength: 32),
                        Gender = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("core.DeceasedPersons", "ObitID");
            AddForeignKey("core.DeceasedPersons", "ObitID", "core.Obits", "ID", cascadeDelete: true);
        }
    }
}
