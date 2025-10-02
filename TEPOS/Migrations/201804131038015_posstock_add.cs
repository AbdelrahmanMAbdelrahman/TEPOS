namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posstock_add : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosStocks", "PosBranchId", c => c.Int(nullable: false));
            CreateIndex("dbo.PosStocks", "PosBranchId");
            AddForeignKey("dbo.PosStocks", "PosBranchId", "dbo.PosBranch", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosStocks", "PosBranchId", "dbo.PosBranch");
            DropIndex("dbo.PosStocks", new[] { "PosBranchId" });
            DropColumn("dbo.PosStocks", "PosBranchId");
        }
    }
}
