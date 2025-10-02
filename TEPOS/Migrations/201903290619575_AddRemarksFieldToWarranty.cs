namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemarksFieldToWarranty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesInvoice", "Remarks", c => c.String(maxLength: 500));
            AddColumn("dbo.PosSalesProductWarrantyIssue", "Rmarks", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosSalesProductWarrantyIssue", "Rmarks");
            DropColumn("dbo.PosSalesInvoice", "Remarks");
        }
    }
}
