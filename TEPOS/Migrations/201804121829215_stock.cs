namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stock : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosStocks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvReferenceNo = c.String(maxLength: 100),
                        CompanyInvNo = c.String(nullable: false, maxLength: 100),
                        PosSupplierId = c.Int(nullable: false),
                        InvDate = c.DateTime(nullable: false, storeType: "date"),
                        InvReceiveDate = c.DateTime(nullable: false, storeType: "date"),
                        OtherDiscount = c.Decimal(precision: 18, scale: 2),
                        LessDiscunt = c.Decimal(precision: 18, scale: 2),
                        TotalTax = c.Decimal(precision: 18, scale: 2),
                        OtherCharges = c.Decimal(precision: 18, scale: 2),
                        Netvalue = c.Decimal(nullable: false, precision: 18, scale: 2,defaultValue:0),
                        NetPayable = c.Decimal(nullable: true, precision: 18, scale: 2,defaultValue:0),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.PosSuppliers", t => t.PosSupplierId, cascadeDelete: false)
                .Index(t => t.PosSupplierId)
                .Index(t => t.CmnCompanyId);
            
            CreateTable(
                "dbo.PosStockDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosProductId = c.Long(nullable: false),
                        PosProductBatchId = c.Long(nullable: false),
                        Qty = c.Long(nullable: false),
                        PosStockTypeId = c.Int(nullable: false),
                        Discount = c.Decimal(precision: 18, scale: 3,nullable:true,defaultValue:0),
                        PurchaseTax = c.Decimal(precision: 18, scale: 3, nullable: true, defaultValue: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PosProducts", t => t.PosProductId, cascadeDelete: false)
                .ForeignKey("dbo.PosProductBatch", t => t.PosProductBatchId, cascadeDelete: false)
                .ForeignKey("dbo.PosStockTypes", t => t.PosStockTypeId, cascadeDelete: false)
                .Index(t => t.PosProductId)
                .Index(t => t.PosProductBatchId)
                .Index(t => t.PosStockTypeId);
            
            CreateTable(
                "dbo.PosStockTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: true)
                .Index(t => t.CmnCompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosStockDetails", "PosStockTypeId", "dbo.PosStockTypes");
            DropForeignKey("dbo.PosStockTypes", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosStockDetails", "PosProductBatchId", "dbo.PosProductBatch");
            DropForeignKey("dbo.PosStockDetails", "PosProductId", "dbo.PosProducts");
            DropForeignKey("dbo.PosStocks", "PosSupplierId", "dbo.PosSuppliers");
            DropForeignKey("dbo.PosStocks", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosStockTypes", new[] { "CmnCompanyId" });
            DropIndex("dbo.PosStockDetails", new[] { "PosStockTypeId" });
            DropIndex("dbo.PosStockDetails", new[] { "PosProductBatchId" });
            DropIndex("dbo.PosStockDetails", new[] { "PosProductId" });
            DropIndex("dbo.PosStocks", new[] { "CmnCompanyId" });
            DropIndex("dbo.PosStocks", new[] { "PosSupplierId" });
            DropTable("dbo.PosStockTypes");
            DropTable("dbo.PosStockDetails");
            DropTable("dbo.PosStocks");
        }
    }
}
