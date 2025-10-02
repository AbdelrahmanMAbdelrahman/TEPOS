namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PosProductBatch : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosProductBatch",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosProductId = c.Long(nullable: false),
                        BatchName = c.String(nullable: false, maxLength: 100),
                        Mrp = c.String(nullable: false, maxLength: 100),
                        PurchaseRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SellingRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateFrom = c.DateTime(),
                        DateTo = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.PosProducts", t => t.PosProductId, cascadeDelete: false)
                .Index(t => new { t.PosProductId, t.BatchName, t.Mrp, t.CmnCompanyId }, unique: true, name: "UK_PosProductId_BatchName_Mrp_CmnCompanyId");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosProductBatch", "PosProductId", "dbo.PosProducts");
            DropForeignKey("dbo.PosProductBatch", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosProductBatch", "UK_PosProductId_BatchName_Mrp_CmnCompanyId");
            DropTable("dbo.PosProductBatch");
        }
    }
}
