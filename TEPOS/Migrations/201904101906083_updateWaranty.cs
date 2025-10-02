namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateWaranty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PosSalesProductWarrantyIssue", "SerialOrImeiNo", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PosSalesProductWarrantyIssue", "SerialOrImeiNo", c => c.String(nullable: false, maxLength: 500));
        }
    }
}
