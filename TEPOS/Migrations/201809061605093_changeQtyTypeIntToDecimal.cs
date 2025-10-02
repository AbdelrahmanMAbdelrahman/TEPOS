namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeQtyTypeIntToDecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CmnCompanies", "VatRegNo", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.PosBillingReportTexts", "PoweredBy", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.PosBillingReportTexts", "Text", c => c.String(nullable: false, maxLength: 2000));
            AlterColumn("dbo.PosSalesInvoiceFreeProducts", "Qty", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PosSalesInvoiceProducts", "Qty", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PosStockDetails", "Qty", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SecUsers", "TerminalId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SecUsers", "TerminalId", c => c.String());
            AlterColumn("dbo.PosStockDetails", "Qty", c => c.Long(nullable: false));
            AlterColumn("dbo.PosSalesInvoiceProducts", "Qty", c => c.Int(nullable: false));
            AlterColumn("dbo.PosSalesInvoiceFreeProducts", "Qty", c => c.Int(nullable: false));
            AlterColumn("dbo.PosBillingReportTexts", "Text", c => c.String(maxLength: 2000));
            AlterColumn("dbo.PosBillingReportTexts", "PoweredBy", c => c.String(maxLength: 200));
            AlterColumn("dbo.CmnCompanies", "VatRegNo", c => c.String(maxLength: 100));
        }
    }
}
