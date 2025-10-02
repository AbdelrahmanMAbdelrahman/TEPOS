namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePosBillingTemplite : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosBillprintTemplateOfBranch", "PosBillprintTemplateOfBranch_CmnCompanyId_PosBranchId");
            CreateIndex("dbo.PosBillprintTemplateOfBranch", "PosBillprintTemplateId");
            CreateIndex("dbo.PosBillprintTemplateOfBranch", new[] { "PosBranchId", "CmnCompanyId" }, unique: true, name: "PosBillprintTemplateOfBranch_CmnCompanyId_PosBranchId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PosBillprintTemplateOfBranch", "PosBillprintTemplateOfBranch_CmnCompanyId_PosBranchId");
            DropIndex("dbo.PosBillprintTemplateOfBranch", new[] { "PosBillprintTemplateId" });
            CreateIndex("dbo.PosBillprintTemplateOfBranch", new[] { "PosBillprintTemplateId", "PosBranchId", "CmnCompanyId" }, unique: true, name: "PosBillprintTemplateOfBranch_CmnCompanyId_PosBranchId");
        }
    }
}
