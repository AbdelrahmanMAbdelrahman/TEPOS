namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewD : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PosProducts", "PosProductCategoryId", "dbo.PosProductCategories");
            DropIndex("dbo.PosProducts", new[] { "PosProductCategoryId" });
            CreateTable(
                "dbo.PosCustomerDueCollections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PosSalesInvoiceId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Date = c.DateTime(nullable: false),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: true)
                .ForeignKey("dbo.PosSalesInvoice", t => t.PosSalesInvoiceId, cascadeDelete: true)
                .Index(t => t.PosSalesInvoiceId)
                .Index(t => t.CmnCompanyId);
            
            AddColumn("dbo.PosSalesInvoice", "IsDuePaid", c => c.Boolean(nullable: false,defaultValue:false));
            AlterColumn("dbo.PosProducts", "PosProductCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.PosProducts", "PosProductCategoryId");
            AddForeignKey("dbo.PosProducts", "PosProductCategoryId", "dbo.PosProductCategories", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosProducts", "PosProductCategoryId", "dbo.PosProductCategories");
            DropForeignKey("dbo.PosCustomerDueCollections", "PosSalesInvoiceId", "dbo.PosSalesInvoice");
            DropForeignKey("dbo.PosCustomerDueCollections", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosProducts", new[] { "PosProductCategoryId" });
            DropIndex("dbo.PosCustomerDueCollections", new[] { "CmnCompanyId" });
            DropIndex("dbo.PosCustomerDueCollections", new[] { "PosSalesInvoiceId" });
            AlterColumn("dbo.PosProducts", "PosProductCategoryId", c => c.Int());
            DropColumn("dbo.PosSalesInvoice", "IsDuePaid");
            DropTable("dbo.PosCustomerDueCollections");
            CreateIndex("dbo.PosProducts", "PosProductCategoryId");
            AddForeignKey("dbo.PosProducts", "PosProductCategoryId", "dbo.PosProductCategories", "Id");
        }
    }
}
