namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUomMaster4 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.PosProducts", "PosSupplierId");
            AddForeignKey("dbo.PosProducts", "PosSupplierId", "dbo.PosSuppliers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosProducts", "PosSupplierId", "dbo.PosSuppliers");
            DropIndex("dbo.PosProducts", new[] { "PosSupplierId" });
        }
    }
}
