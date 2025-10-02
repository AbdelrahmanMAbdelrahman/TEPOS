namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dropPosconstraintSeting : DbMigration
    {
        public override void Up()
        {
           
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SttConstPosInvoiceTypeNotInForStockSides",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Description = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SttConstPosInvoiceTypeNotInForSalesSide",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Description = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
