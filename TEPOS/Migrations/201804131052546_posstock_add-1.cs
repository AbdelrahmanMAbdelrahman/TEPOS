namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posstock_add1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosStocks", "UK_PosStock_InvReferenceNo_CmnCmpanyId");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.PosStocks", new[] { "InvReferenceNo", "CmnCompanyId" }, unique: true, name: "UK_PosStock_InvReferenceNo_CmnCmpanyId");
        }
    }
}
