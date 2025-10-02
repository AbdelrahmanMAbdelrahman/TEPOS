namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWarrantyIssue : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosProductBatch", "UK_PosProductBatch_BarCode_CompanyId");
            CreateTable(
                "dbo.PosSalesProductWarrantyIssue",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosSalesInvoiceProductId = c.Long(nullable: false),
                        SerialOrImeiNo = c.String(nullable: false, maxLength: 200),
                        WarrantyPeriod = c.Int(nullable: false),
                        WarrantyType = c.Int(nullable: false),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.PosSalesInvoiceProducts", t => t.PosSalesInvoiceProductId, cascadeDelete: false)
                .Index(t => t.PosSalesInvoiceProductId)
                .Index(t => t.CmnCompanyId);
            
                  }
        
        public override void Down()
        {
            DropTable("dbo.PosSalesProductWarrantyIssue");
            CreateIndex("dbo.PosProductBatch", new[] { "BarCode", "CmnCompanyId" }, unique: true, name: "UK_PosProductBatch_BarCode_CompanyId");
        }
    }
}
