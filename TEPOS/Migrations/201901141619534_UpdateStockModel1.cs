namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStockModel1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosStocks", "UK_PosStock_CompanyInvNo_CmnCmpanyId");
            CreateIndex("dbo.PosStocks", new[] { "CompanyInvNo", "CmnCompanyId", "PosStockTransactionType" }, unique: true, name: "UK_PosStock_CompanyInvNo_PosStockTransactionType_CmnCmpanyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PosStocks", "UK_PosStock_CompanyInvNo_PosStockTransactionType_CmnCmpanyId");
            CreateIndex("dbo.PosStocks", new[] { "CompanyInvNo", "CmnCompanyId" }, unique: true, name: "UK_PosStock_CompanyInvNo_CmnCmpanyId");
        }
    }
}
