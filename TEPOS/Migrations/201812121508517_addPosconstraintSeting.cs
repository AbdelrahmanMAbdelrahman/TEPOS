namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPosconstraintSeting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SttConstPosInvoiceTypeNotInForSalesSides",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Description = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SttConstPosInvoiceTypeNotInForStockSides",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Description = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SttConstPosInvoiceTypeNotInForStockSides");
            DropTable("dbo.SttConstPosInvoiceTypeNotInForSalesSides");
        }
    }
}
