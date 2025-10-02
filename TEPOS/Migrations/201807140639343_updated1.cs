namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updated1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosScheme", "UK_PosScheme_SchemeCode_CmnCompanyId");
            AlterColumn("dbo.PosScheme", "SchemeCode", c => c.Long(nullable: false));
            CreateIndex("dbo.PosSalesInvoiceProducts", "PosProductId");
            CreateIndex("dbo.PosSalesInvoiceProducts", "PosProductBatchId");
            CreateIndex("dbo.PosScheme", new[] { "SchemeCode", "CmnCompanyId" }, unique: true, name: "UK_PosScheme_SchemeCode_CmnCompanyId");
            AddForeignKey("dbo.PosSalesInvoiceProducts", "PosProductId", "dbo.PosProducts", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosSalesInvoiceProducts", "PosProductBatchId", "dbo.PosProductBatch", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosSalesInvoiceProducts", "PosProductBatchId", "dbo.PosProductBatch");
            DropForeignKey("dbo.PosSalesInvoiceProducts", "PosProductId", "dbo.PosProducts");
            DropIndex("dbo.PosScheme", "UK_PosScheme_SchemeCode_CmnCompanyId");
            DropIndex("dbo.PosSalesInvoiceProducts", new[] { "PosProductBatchId" });
            DropIndex("dbo.PosSalesInvoiceProducts", new[] { "PosProductId" });
            AlterColumn("dbo.PosScheme", "SchemeCode", c => c.Long(nullable: false));
            CreateIndex("dbo.PosScheme", new[] { "SchemeCode", "CmnCompanyId" }, unique: true, name: "UK_PosScheme_SchemeCode_CmnCompanyId");
        }
    }
}
