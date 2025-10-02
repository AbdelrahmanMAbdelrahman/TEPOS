namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewColumnBranchToCustomer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosCustomers", "PosBranchId", c => c.Int(nullable: true));
            AlterColumn("dbo.PosCustomers", "CustomerNo", c => c.Long(nullable: false));
            CreateIndex("dbo.PosCustomers", "PosBranchId");
            AddForeignKey("dbo.PosCustomers", "PosBranchId", "dbo.PosBranch", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosCustomers", "PosBranchId", "dbo.PosBranch");
            DropIndex("dbo.PosCustomers", new[] { "PosBranchId" });
            DropIndex("dbo.PosCustomers", "UK_PosCustomer_CustomerNo_CmnCompanyId");
            AlterColumn("dbo.PosCustomers", "CustomerNo", c => c.Long(nullable: false));
            DropColumn("dbo.PosCustomers", "PosBranchId");
            CreateIndex("dbo.PosCustomers", new[] { "CustomerNo", "CmnCompanyId" }, unique: true, name: "UK_PosCustomer_CustomerNo_CmnCompanyId");
        }
    }
}
