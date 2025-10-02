namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtender : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosTenders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250),
                        Order = c.Int(nullable: false),
                        ConstraintType = c.String(maxLength: 2),
                        PosTenderTypeId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.PosTenderTypes", t => t.PosTenderTypeId, cascadeDelete: false)
                .Index(t => t.PosTenderTypeId)
                .Index(t => t.CmnCompanyId);
            
            CreateTable(
                "dbo.PosTenderTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .Index(t => t.CmnCompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosTenders", "PosTenderTypeId", "dbo.PosTenderTypes");
            DropForeignKey("dbo.PosTenderTypes", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosTenders", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosTenderTypes", new[] { "CmnCompanyId" });
            DropIndex("dbo.PosTenders", new[] { "CmnCompanyId" });
            DropIndex("dbo.PosTenders", new[] { "PosTenderTypeId" });
            DropTable("dbo.PosTenderTypes");
            DropTable("dbo.PosTenders");
        }
    }
}
