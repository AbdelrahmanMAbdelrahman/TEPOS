namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supplier_product : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosSuppliers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 1000),
                        Address = c.String(nullable: false),
                        Phone = c.String(maxLength: 100),
                        Fax = c.String(maxLength: 100),
                        Email = c.String(maxLength: 200),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, false, "FK_PosSupplier_CmnCompanyId")
                .Index(t => t.CmnCompanyId);
            
            AddColumn("dbo.PosProducts", "CustomCode", c => c.String(nullable: false, maxLength: 250));
            AddColumn("dbo.PosProducts", "ShortName", c => c.String(nullable: false, maxLength: 1000));
            AddColumn("dbo.PosProducts", "PosSupplierId", c => c.Int());
            CreateIndex("dbo.PosProducts", new[] { "Code", "CustomCode", "CmnCompanyId" }, unique: true, name: "UK_PosProduct_Code_CustomCode_CmnCompanyId");
            CreateIndex("dbo.PosProducts", new[] { "Code", "ShortName", "CmnCompanyId" }, unique: true, name: "UK_PosProduct_Code_ShortName_CmnCompanyId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosSuppliers", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosSuppliers", new[] { "CmnCompanyId" });
            DropIndex("dbo.PosProducts", "UK_PosProduct_Code_ShortName_CmnCompanyId");
            DropIndex("dbo.PosProducts", "UK_PosProduct_Code_CustomCode_CmnCompanyId");
            DropColumn("dbo.PosProducts", "PosSupplierId");
            DropColumn("dbo.PosProducts", "ShortName");
            DropColumn("dbo.PosProducts", "CustomCode");
            DropTable("dbo.PosSuppliers");
        }
    }
}
