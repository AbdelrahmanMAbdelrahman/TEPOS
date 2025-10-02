namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMigration22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesInvoice", "SdTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PosProducts", "Sd", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PosSalesInvoiceProducts", "Sd", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.PosSalesInvoice", "TaxTotal");
            DropColumn("dbo.PosProducts", "Tax");
            DropColumn("dbo.PosSalesInvoiceProducts", "Tax");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosSalesInvoiceProducts", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PosProducts", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PosSalesInvoice", "TaxTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.PosSalesInvoiceProducts", "Sd");
            DropColumn("dbo.PosProducts", "Sd");
            DropColumn("dbo.PosSalesInvoice", "SdTotal");
        }
    }
}
