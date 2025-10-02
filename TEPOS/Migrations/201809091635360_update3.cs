namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class update3 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.PosSalesInvoiceFreeProducts", "PosProductId");
            CreateIndex("dbo.PosSalesInvoiceFreeProducts", "PosProductBatchId");
            AddForeignKey("dbo.PosSalesInvoiceFreeProducts", "PosProductId", "dbo.PosProducts", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosSalesInvoiceFreeProducts", "PosProductBatchId", "dbo.PosProductBatch", "Id", cascadeDelete: false);
        }

        public override void Down()
        {
            DropForeignKey("dbo.PosSalesInvoiceFreeProducts", "PosProductBatchId", "dbo.PosProductBatch");
            DropForeignKey("dbo.PosSalesInvoiceFreeProducts", "PosProductId", "dbo.PosProducts");
            DropIndex("dbo.PosSalesInvoiceFreeProducts", new[] {"PosProductBatchId"});
            DropIndex("dbo.PosSalesInvoiceFreeProducts", new[] {"PosProductId"});

        }
    }
}
