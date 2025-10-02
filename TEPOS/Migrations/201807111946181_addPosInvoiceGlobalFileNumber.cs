namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPosInvoiceGlobalFileNumber : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosInvoiceGlobalFileNumber",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceGlobalFileNumber = c.Long(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .Index(t => t.InvoiceGlobalFileNumber, unique: true, name: "UK_PosInvoiceGlobalFileNumber_InvoiceGlobalFileNumber")
                .Index(t => t.CmnCompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosInvoiceGlobalFileNumber", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosInvoiceGlobalFileNumber", new[] { "CmnCompanyId" });
            DropIndex("dbo.PosInvoiceGlobalFileNumber", "UK_PosInvoiceGlobalFileNumber_InvoiceGlobalFileNumber");
            DropTable("dbo.PosInvoiceGlobalFileNumber");
        }
    }
}
