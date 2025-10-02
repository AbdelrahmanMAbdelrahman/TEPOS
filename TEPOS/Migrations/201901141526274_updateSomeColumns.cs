namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateSomeColumns : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosProductCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: true)
                .Index(t => new { t.Name, t.CmnCompanyId }, unique: true, name: "UK_PosCategory_Name_CmnCompanyId");
            
            AddColumn("dbo.PosProducts", "PosProductCategoryId", c => c.Int());
            AddColumn("dbo.PosStocks", "Remarks", c => c.String(maxLength: 1000));
            AddColumn("dbo.PosStocks", "PosStockTransactionType", c => c.Int(nullable: false,defaultValue:1));
            CreateIndex("dbo.PosProducts", "PosProductCategoryId");
            AddForeignKey("dbo.PosProducts", "PosProductCategoryId", "dbo.PosProductCategories", "Id");
        }
        
        public override void Down()
        {
           DropForeignKey("dbo.PosProducts", "PosProductCategoryId", "dbo.PosProductCategories");
            DropForeignKey("dbo.PosProductCategories", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosProductCategories", "UK_PosCategory_Name_CmnCompanyId");
            DropIndex("dbo.PosProducts", new[] { "PosProductCategoryId" });
            DropColumn("dbo.PosStocks", "PosStockTransactionType");
            DropColumn("dbo.PosStocks", "Remarks");
            DropColumn("dbo.PosProducts", "PosProductCategoryId");
            DropTable("dbo.PosProductCategories");
        }
    }
}
