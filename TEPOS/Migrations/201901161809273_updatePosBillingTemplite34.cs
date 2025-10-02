namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePosBillingTemplite34 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosBillprintTemplateOfBranch", new[] { "PosBillprintTemplateId" });
            DropIndex("dbo.PosBillprintTemplateOfBranch", "PosBillprintTemplateOfBranch_CmnCompanyId_PosBranchId");
            CreateIndex("dbo.PosBillprintTemplateOfBranch", new[] { "PosBillprintTemplateId", "PosBranchId", "CmnCompanyId" }, unique: true, name: "UK_PosBillprintTemplateOfBranch_CmnCompanyId_PosBranchId_PosBillprintTemplateId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PosBillprintTemplateOfBranch", "UK_PosBillprintTemplateOfBranch_CmnCompanyId_PosBranchId_PosBillprintTemplateId");
            CreateIndex("dbo.PosBillprintTemplateOfBranch", new[] { "PosBranchId", "CmnCompanyId" }, unique: true, name: "PosBillprintTemplateOfBranch_CmnCompanyId_PosBranchId");
            CreateIndex("dbo.PosBillprintTemplateOfBranch", "PosBillprintTemplateId");
        }
    }
}
