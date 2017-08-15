namespace SamClientDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DisplayEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Displays",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ConsolationID = c.Int(nullable: false),
                        TimeOfDisplay = c.DateTime(nullable: false),
                        DurationMilliSeconds = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Consolations", t => t.ConsolationID, cascadeDelete: true)
                .Index(t => t.ConsolationID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Displays", "ConsolationID", "dbo.Consolations");
            DropIndex("dbo.Displays", new[] { "ConsolationID" });
            DropTable("dbo.Displays");
        }
    }
}
