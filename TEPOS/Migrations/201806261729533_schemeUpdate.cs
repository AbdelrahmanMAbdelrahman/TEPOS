namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class schemeUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosCustomerClass",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable:false,maxLength:100),
                        Status = c.Boolean(nullable: false),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .Index(t => new { t.Name, t.CmnCompanyId }, unique: true, name: "UK_PosCustomerClass_Name_CmnCompanyId");
            
            CreateTable(
                "dbo.PosSchemeBranch",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosSchemeId = c.Long(nullable: false),
                        PosBranchId = c.Long(nullable: false),
                        PosBranch_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PosBranch", t => t.PosBranch_Id)
                .ForeignKey("dbo.PosScheme", t => t.PosSchemeId, cascadeDelete: false)
                .Index(t => new { t.PosSchemeId, t.PosBranchId }, unique: true, name: "UK_PosSchemeBranch_PosSchemeId")
                .Index(t => t.PosBranch_Id);
            
            CreateTable(
                "dbo.PosScheme",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SchemeCode = c.Long(nullable: false),
                        Description = c.String(maxLength: 500),
                        IsCombiScheme = c.Boolean(nullable: false),
                        SchemeType = c.Int(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .Index(t => new { t.SchemeCode, t.CmnCompanyId }, unique: true, name: "UK_PosScheme_SchemeCode_CmnCompanyId");
            
            CreateTable(
                "dbo.PosSchemeCustomerClass",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosSchemeId = c.Long(nullable: false),
                        PosCustomerClassId = c.Long(nullable: false),
                        PosCustomerClass_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PosCustomerClass", t => t.PosCustomerClass_Id)
                .ForeignKey("dbo.PosScheme", t => t.PosSchemeId, cascadeDelete: false)
                .Index(t => new { t.PosSchemeId, t.PosCustomerClassId }, unique: true, name: "UK_PosSchemeCustomerClass_PosCustomerClassId_PosSchemeId")
                .Index(t => t.PosCustomerClass_Id);
            
            CreateTable(
                "dbo.PosSchemeProduct",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosSchemeId = c.Long(nullable: false),
                        PosProductId = c.Long(nullable: false),
                        PosProductBatchId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PosProducts", t => t.PosProductId, cascadeDelete: false)
                .ForeignKey("dbo.PosProductBatch", t => t.PosProductBatchId, cascadeDelete: false)
                .ForeignKey("dbo.PosScheme", t => t.PosSchemeId, cascadeDelete: false)
                .Index(t => new { t.PosSchemeId, t.PosProductId, t.PosProductBatchId }, unique: true, name: "UK_PosSchemeProduct_PosSchemeId_PosProductId_PosProductBatchId");
            
            CreateTable(
                "dbo.PosSchemeSlabs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosSchemeId = c.Long(nullable: false),
                        PurQtyOrAmt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPer = c.Int(nullable: false),
                        FlatAmt = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PosScheme", t => t.PosSchemeId, cascadeDelete: false)
                .Index(t => t.PosSchemeId);
            
            CreateTable(
                "dbo.PosSchemeSlabFreeProducts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosSchemeId = c.Long(nullable: false),
                        PosSchemeSlabId = c.Long(nullable: false),
                        PosProductId = c.Long(nullable: false),
                        PosProductBatchId = c.Long(nullable: false),
                        Qty = c.Int(nullable: false),
                        PosUomMasterId = c.Int(nullable: false),
                        Availability = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PosProducts", t => t.PosProductId, cascadeDelete: false)
                .ForeignKey("dbo.PosProductBatch", t => t.PosProductBatchId, cascadeDelete: false)
                .ForeignKey("dbo.PosScheme", t => t.PosSchemeId, cascadeDelete: false)
                .ForeignKey("dbo.PosSchemeSlabs", t => t.PosSchemeSlabId, cascadeDelete: false)
                .ForeignKey("dbo.PosUomMaster", t => t.PosUomMasterId, cascadeDelete: false)
                .Index(t => new { t.PosSchemeId, t.PosSchemeSlabId, t.PosProductId, t.PosProductBatchId }, unique: true, name: "UK_PosSchemeSlabFreeProduct_PosSchemeId_PosSchemeSlabId_PosProductId_PosProductBatchId")
                .Index(t => t.PosUomMasterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosSchemeSlabFreeProducts", "PosUomMasterId", "dbo.PosUomMaster");
            DropForeignKey("dbo.PosSchemeSlabFreeProducts", "PosSchemeSlabId", "dbo.PosSchemeSlabs");
            DropForeignKey("dbo.PosSchemeSlabFreeProducts", "PosSchemeId", "dbo.PosScheme");
            DropForeignKey("dbo.PosSchemeSlabFreeProducts", "PosProductBatchId", "dbo.PosProductBatch");
            DropForeignKey("dbo.PosSchemeSlabFreeProducts", "PosProductId", "dbo.PosProducts");
            DropForeignKey("dbo.PosSchemeSlabs", "PosSchemeId", "dbo.PosScheme");
            DropForeignKey("dbo.PosSchemeProduct", "PosSchemeId", "dbo.PosScheme");
            DropForeignKey("dbo.PosSchemeProduct", "PosProductBatchId", "dbo.PosProductBatch");
            DropForeignKey("dbo.PosSchemeProduct", "PosProductId", "dbo.PosProducts");
            DropForeignKey("dbo.PosSchemeCustomerClass", "PosSchemeId", "dbo.PosScheme");
            DropForeignKey("dbo.PosSchemeCustomerClass", "PosCustomerClass_Id", "dbo.PosCustomerClass");
            DropForeignKey("dbo.PosSchemeBranch", "PosSchemeId", "dbo.PosScheme");
            DropForeignKey("dbo.PosScheme", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosSchemeBranch", "PosBranch_Id", "dbo.PosBranch");
            DropForeignKey("dbo.PosCustomerClass", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosSchemeSlabFreeProducts", new[] { "PosUomMasterId" });
            DropIndex("dbo.PosSchemeSlabFreeProducts", "UK_PosSchemeSlabFreeProduct_PosSchemeId_PosSchemeSlabId_PosProductId_PosProductBatchId");
            DropIndex("dbo.PosSchemeSlabs", new[] { "PosSchemeId" });
            DropIndex("dbo.PosSchemeProduct", "UK_PosSchemeProduct_PosSchemeId_PosProductId_PosProductBatchId");
            DropIndex("dbo.PosSchemeCustomerClass", new[] { "PosCustomerClass_Id" });
            DropIndex("dbo.PosSchemeCustomerClass", "UK_PosSchemeCustomerClass_PosCustomerClassId_PosSchemeId");
            DropIndex("dbo.PosScheme", "UK_PosScheme_SchemeCode_CmnCompanyId");
            DropIndex("dbo.PosSchemeBranch", new[] { "PosBranch_Id" });
            DropIndex("dbo.PosSchemeBranch", "UK_PosSchemeBranch_PosSchemeId");
            DropIndex("dbo.PosCustomerClass", "UK_PosCustomerClass_Name_CmnCompanyId");
            DropTable("dbo.PosSchemeSlabFreeProducts");
            DropTable("dbo.PosSchemeSlabs");
            DropTable("dbo.PosSchemeProduct");
            DropTable("dbo.PosSchemeCustomerClass");
            DropTable("dbo.PosScheme");
            DropTable("dbo.PosSchemeBranch");
            DropTable("dbo.PosCustomerClass");
        }
    }
}
