namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUserTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosBillingReportTexts", "PoweredBy", c => c.String(maxLength: 200));
            AddColumn("dbo.SecUsers", "TerminalId", c => c.String());
            DropColumn("dbo.PosBillingReportTexts", "Type");
            DropColumn("dbo.SecUsers", "Flag");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SecUsers", "Flag", c => c.Boolean(nullable: false));
            AddColumn("dbo.PosBillingReportTexts", "Type", c => c.String(maxLength: 20));
            DropColumn("dbo.SecUsers", "TerminalId");
            DropColumn("dbo.PosBillingReportTexts", "PoweredBy");
        }
    }
}
