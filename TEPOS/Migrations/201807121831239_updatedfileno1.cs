namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedfileno1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId", "dbo.PosInvoiceGlobalFileNumber");
            DropIndex("dbo.PosSalesInvoice", new[] { "PosInvoiceGlobalFileNumberId" });
            AlterColumn("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId", c => c.Long(nullable: false));
            CreateIndex("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId");
            AddForeignKey("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId", "dbo.PosInvoiceGlobalFileNumber", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId", "dbo.PosInvoiceGlobalFileNumber");
            DropIndex("dbo.PosSalesInvoice", new[] { "PosInvoiceGlobalFileNumberId" });
            AlterColumn("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId", c => c.Long());
            CreateIndex("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId");
            AddForeignKey("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId", "dbo.PosInvoiceGlobalFileNumber", "Id");
        }
    }
}
