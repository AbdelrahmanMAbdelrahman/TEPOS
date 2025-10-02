namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTender : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosTenders", new[] { "CmnCompanyId" });
            DropIndex("dbo.PosTenderTypes", new[] { "CmnCompanyId" });
            CreateIndex("dbo.PosTenders", new[] { "Name", "ConstraintType", "CmnCompanyId" }, unique: true, name: "UK_PosTenders_Name_ConstraintType_CmnCompanyId");
            CreateIndex("dbo.PosTenderTypes", new[] { "Name", "CmnCompanyId" }, unique: true, name: "UK_PosTenderTypes_Name_CmnCompanyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PosTenderTypes", "UK_PosTenderTypes_Name_CmnCompanyId");
            DropIndex("dbo.PosTenders", "UK_PosTenders_Name_ConstraintType_CmnCompanyId");
            CreateIndex("dbo.PosTenderTypes", "CmnCompanyId");
            CreateIndex("dbo.PosTenders", "CmnCompanyId");
        }
    }
}
