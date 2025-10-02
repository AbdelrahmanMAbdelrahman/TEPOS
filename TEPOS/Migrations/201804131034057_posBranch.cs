namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posBranch : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosStocks", new[] { "CmnCompanyId" });
            CreateTable(
                "dbo.PosBranch",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        Address = c.String(nullable: false, maxLength: 500),
                        Phone = c.String(maxLength: 100),
                        Mobile = c.String(maxLength: 100),
                        Fax = c.String(maxLength: 100),
                        Remarks = c.String(maxLength: 500),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .Index(t => new { t.Name, t.CmnCompanyId }, unique: true, name: "UK_PosBranch_Name_CmnCmpanyId");
            
            CreateIndex("dbo.PosStocks", new[] { "InvReferenceNo", "CmnCompanyId" }, unique: true, name: "UK_PosStock_InvReferenceNo_CmnCmpanyId");
            CreateIndex("dbo.PosStocks", new[] { "CompanyInvNo", "CmnCompanyId" }, unique: true, name: "UK_PosStock_CompanyInvNo_CmnCmpanyId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosBranch", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosStocks", "UK_PosStock_CompanyInvNo_CmnCmpanyId");
            DropIndex("dbo.PosStocks", "UK_PosStock_InvReferenceNo_CmnCmpanyId");
            DropIndex("dbo.PosBranch", "UK_PosBranch_Name_CmnCmpanyId");
            DropTable("dbo.PosBranch");
            CreateIndex("dbo.PosStocks", "CmnCompanyId");
        }
    }
}
