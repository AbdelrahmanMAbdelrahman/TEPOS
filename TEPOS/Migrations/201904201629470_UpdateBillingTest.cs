namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateBillingTest : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PosBillingReportTexts", "Text", c => c.String(nullable: false, maxLength: 4000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PosBillingReportTexts", "Text", c => c.String(nullable: false, maxLength: 2000));
        }
    }
}
