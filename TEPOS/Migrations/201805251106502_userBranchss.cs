namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userBranchss : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SecUserBranch",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SecUserId = c.Int(nullable: false),
                        PosBranchId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.PosBranch", t => t.PosBranchId, cascadeDelete: false)
                .Index(t => t.PosBranchId)
                .Index(t => t.CmnCompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SecUserBranch", "PosBranchId", "dbo.PosBranch");
            DropForeignKey("dbo.SecUserBranch", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.SecUserBranch", new[] { "CmnCompanyId" });
            DropIndex("dbo.SecUserBranch", new[] { "PosBranchId" });
            DropTable("dbo.SecUserBranch");
        }
    }
}
