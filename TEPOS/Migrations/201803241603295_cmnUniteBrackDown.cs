namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cmnUniteBrackDown : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CmnUnitBreakDown",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        BigUnitName = c.String(nullable: false, maxLength: 20),
                        BigUnitValue = c.Int(nullable: false),
                        SmallUnitName = c.String(nullable: false, maxLength: 20),
                        SmallUnitValue = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, false, "FK_CmnUnitBreakDown_CmnCompanyId")
                .Index(t => t.CmnCompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CmnUnitBreakDown", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.CmnUnitBreakDown", new[] { "CmnCompanyId" });
            DropTable("dbo.CmnUnitBreakDown");
        }
    }
}
