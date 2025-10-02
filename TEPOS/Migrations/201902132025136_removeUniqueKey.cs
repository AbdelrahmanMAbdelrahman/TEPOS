namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeUniqueKey : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosProductBatch", "UK_PosProductBatch_BarCode_CompanyId");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.PosProductBatch", new[] { "BarCode", "CmnCompanyId" }, unique: true, name: "UK_PosProductBatch_BarCode_CompanyId");
        }
    }
}
