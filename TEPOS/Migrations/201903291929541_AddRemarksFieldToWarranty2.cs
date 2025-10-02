namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemarksFieldToWarranty2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesProductWarrantyIssue", "Remarks", c => c.String(maxLength: 250));
            AlterColumn("dbo.PosSalesProductWarrantyIssue", "SerialOrImeiNo", c => c.String(nullable: false, maxLength: 500));
            DropColumn("dbo.PosSalesProductWarrantyIssue", "Rmarks");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosSalesProductWarrantyIssue", "Rmarks", c => c.String(maxLength: 250));
            AlterColumn("dbo.PosSalesProductWarrantyIssue", "SerialOrImeiNo", c => c.String(nullable: false, maxLength: 200));
            DropColumn("dbo.PosSalesProductWarrantyIssue", "Remarks");
        }
    }
}
