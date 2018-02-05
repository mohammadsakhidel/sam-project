namespace SamDataAccess.Migrations.SamDbMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemParameters_LastDisplayReportDateFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SystemParameters", "LastDisplayReportDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SystemParameters", "LastDisplayReportDate");
        }
    }
}
