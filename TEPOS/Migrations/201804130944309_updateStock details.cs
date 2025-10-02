namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateStockdetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosStockDetails", "PosStockId", c => c.Long(nullable: false));
            CreateIndex("dbo.PosStockDetails", "PosStockId");
            AddForeignKey("dbo.PosStockDetails", "PosStockId", "dbo.PosStocks", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosStockDetails", "PosStockId", "dbo.PosStocks");
            DropIndex("dbo.PosStockDetails", new[] { "PosStockId" });
            DropColumn("dbo.PosStockDetails", "PosStockId");
        }
    }
}
