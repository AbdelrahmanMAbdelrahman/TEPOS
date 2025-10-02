namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSomefireld : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosScheme", "UK_PosScheme_SchemeCode_CmnCompanyId");
            CreateTable(
                "dbo.PosBillingReportTexts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PosBranchId = c.Int(nullable: false),
                        Type = c.String(maxLength: 20),
                        Text = c.String(maxLength: 2000),
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
            
            AddColumn("dbo.CmnCompanies", "VatRegNo", c => c.String(maxLength: 100));
            AddColumn("dbo.PosBranch", "Email", c => c.String(maxLength: 100));
            AlterColumn("dbo.PosCustomers", "Phone", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.PosScheme", "SchemeCode", c => c.Long(nullable: false));
            CreateIndex("dbo.PosScheme", new[] { "SchemeCode", "CmnCompanyId" }, unique: true, name: "UK_PosScheme_SchemeCode_CmnCompanyId");
            DropColumn("dbo.SecUsers", "VisibleExtraOt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SecUsers", "VisibleExtraOt", c => c.Int(nullable: false));
            DropForeignKey("dbo.PosBillingReportTexts", "PosBranchId", "dbo.PosBranch");
            DropForeignKey("dbo.PosBillingReportTexts", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosScheme", "UK_PosScheme_SchemeCode_CmnCompanyId");
            DropIndex("dbo.PosBillingReportTexts", new[] { "CmnCompanyId" });
            DropIndex("dbo.PosBillingReportTexts", new[] { "PosBranchId" });
            AlterColumn("dbo.PosScheme", "SchemeCode", c => c.Long(nullable: false));
            AlterColumn("dbo.PosCustomers", "Phone", c => c.String(maxLength: 20));
            DropColumn("dbo.PosBranch", "Email");
            DropColumn("dbo.CmnCompanies", "VatRegNo");
            DropTable("dbo.PosBillingReportTexts");
            CreateIndex("dbo.PosScheme", new[] { "SchemeCode", "CmnCompanyId" }, unique: true, name: "UK_PosScheme_SchemeCode_CmnCompanyId");
        }
    }
}
