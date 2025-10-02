namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class billprintTemplate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosBillprintTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Image = c.String(),
                        Remarks = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PosBillprintTemplateOfBranch",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PosBillprintTemplateId = c.Int(nullable: false),
                        PosBranchId = c.Int(nullable: false),
                        Remarks = c.Int(nullable: false),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.PosBillprintTemplate", t => t.PosBillprintTemplateId, cascadeDelete: false)
                .Index(t => new { t.PosBillprintTemplateId, t.PosBranchId, t.CmnCompanyId }, unique: true, name: "PosBillprintTemplateOfBranch_CmnCompanyId_PosBranchId");
            
 }
        
        public override void Down()
        {
            AddColumn("dbo.PosProducts", "PosCategoryId", c => c.Int(nullable: false));
            DropForeignKey("dbo.PosBillprintTemplateOfBranch", "PosBillprintTemplateId", "dbo.PosBillprintTemplate");
            DropForeignKey("dbo.PosBillprintTemplateOfBranch", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosBillprintTemplateOfBranch", "PosBillprintTemplateOfBranch_CmnCompanyId_PosBranchId");
            DropTable("dbo.PosBillprintTemplateOfBranch");
            DropTable("dbo.PosBillprintTemplate");
        }
    }
}
