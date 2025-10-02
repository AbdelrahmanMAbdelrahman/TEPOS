namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpadateSDToTax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesInvoice", "TaxTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2,defaultValue:0));
            AddColumn("dbo.PosSalesInvoice", "VatTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2,defaultValue:0));
            AddColumn("dbo.PosSalesInvoiceProducts", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2,defaultValue:0));
            DropColumn("dbo.PosSalesInvoice", "SdTotal");
            DropColumn("dbo.PosSalesInvoice", "VatOrTax");
            DropColumn("dbo.PosSalesInvoiceProducts", "SD");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosSalesInvoiceProducts", "SD", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PosSalesInvoice", "VatOrTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PosSalesInvoice", "SdTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.PosSalesInvoiceProducts", "Tax");
            DropColumn("dbo.PosSalesInvoice", "VatTotal");
            DropColumn("dbo.PosSalesInvoice", "TaxTotal");
        }
    }
}
