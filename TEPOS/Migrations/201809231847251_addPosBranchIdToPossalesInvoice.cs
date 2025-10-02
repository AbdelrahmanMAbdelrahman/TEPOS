namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPosBranchIdToPossalesInvoice : DbMigration
    {
        public override void Up()
        {

            DropIndex("dbo.PosSchemeBranch", "UK_PosSchemeBranch_PosSchemeId");
            DropIndex("dbo.PosSchemeBranch", new[] { "PosBranch_Id" });
            AddColumn("dbo.PosSalesInvoice", "PosBranchId", c => c.Int(nullable: false,defaultValue:1));
            AddForeignKey("dbo.PosSalesInvoice", "PosBranchId", "dbo.PosBranch", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosSchemeBranch", "PosBranchId", "dbo.PosBranch", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosVwPurchaseReceipt", "PosUomMaster_Id", c => c.Int());
            DropForeignKey("dbo.PosSchemeBranch", "PosBranchId", "dbo.PosBranch");
            DropForeignKey("dbo.PosVwPurchaseReceipt", "PosStockId", "dbo.PosStocks");
            DropForeignKey("dbo.PosSalesInvoice", "PosBranchId", "dbo.PosBranch");
            DropIndex("dbo.PosSchemeBranch", "UK_PosSchemeBranch_PosSchemeId");
            DropIndex("dbo.PosVwPurchaseReceipt", new[] { "PosStockId" });
            DropIndex("dbo.PosSalesInvoice", new[] { "PosBranchId" });
            AlterColumn("dbo.PosSchemeBranch", "PosBranchId", c => c.Int());
            AlterColumn("dbo.PosSchemeBranch", "PosBranchId", c => c.Long(nullable: false));
            AlterColumn("dbo.PosVwPurchaseReceipt", "PosStockId", c => c.Long());
            AlterColumn("dbo.PosVwPurchaseReceipt", "PosStockId", c => c.Int(nullable: false));
            DropColumn("dbo.PosSalesInvoice", "PosBranchId");
            RenameColumn(table: "dbo.PosSchemeBranch", name: "PosBranchId", newName: "PosBranch_Id");
            RenameColumn(table: "dbo.PosVwPurchaseReceipt", name: "PosStockId", newName: "PosStock_Id");
            AddColumn("dbo.PosSchemeBranch", "PosBranchId", c => c.Long(nullable: false));
            AddColumn("dbo.PosVwPurchaseReceipt", "PosStockId", c => c.Int(nullable: false));
            CreateIndex("dbo.PosSchemeBranch", "PosBranch_Id");
            CreateIndex("dbo.PosSchemeBranch", new[] { "PosSchemeId", "PosBranchId" }, unique: true, name: "UK_PosSchemeBranch_PosSchemeId");
            CreateIndex("dbo.PosVwPurchaseReceipt", "PosUomMaster_Id");
            CreateIndex("dbo.PosVwPurchaseReceipt", "PosStock_Id");
            AddForeignKey("dbo.PosSchemeBranch", "PosBranch_Id", "dbo.PosBranch", "Id");
            AddForeignKey("dbo.PosVwPurchaseReceipt", "PosStock_Id", "dbo.PosStocks", "Id");
            AddForeignKey("dbo.PosVwPurchaseReceipt", "PosUomMaster_Id", "dbo.PosUomMaster", "Id");
        }
    }
}
