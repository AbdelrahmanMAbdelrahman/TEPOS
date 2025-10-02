namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dasda : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosSalesInvoice", "UK_PosSalesInvoice_InvoiceNumber_CmnCompanyId");
            AlterColumn("dbo.PosSalesInvoice", "InvoiceNumber", c => c.Long(nullable: false));
            CreateIndex("dbo.PosSalesInvoice", new[] { "InvoiceNumber", "CmnCompanyId" }, unique: true, name: "UK_PosSalesInvoice_InvoiceNumber_CmnCompanyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PosSalesInvoice", "UK_PosSalesInvoice_InvoiceNumber_CmnCompanyId");
            AlterColumn("dbo.PosSalesInvoice", "InvoiceNumber", c => c.Long(nullable: false));
            CreateIndex("dbo.PosSalesInvoice", new[] { "InvoiceNumber", "CmnCompanyId" }, unique: true, name: "UK_PosSalesInvoice_InvoiceNumber_CmnCompanyId");
        }
    }
}
