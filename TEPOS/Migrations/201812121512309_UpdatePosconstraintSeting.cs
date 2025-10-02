namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePosconstraintSeting : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.SttConstPosInvoiceTypeNotInForSalesSides", newName: "SttConstPosInvoiceTypeNotInForSalesSide");
            AlterColumn("dbo.SttConstPosInvoiceTypeNotInForSalesSide", "Description", c => c.String(maxLength: 250));
            AlterColumn("dbo.SttConstPosInvoiceTypeNotInForStockSides", "Description", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SttConstPosInvoiceTypeNotInForStockSides", "Description", c => c.Int(nullable: false));
            AlterColumn("dbo.SttConstPosInvoiceTypeNotInForSalesSide", "Description", c => c.Int(nullable: false));
            RenameTable(name: "dbo.SttConstPosInvoiceTypeNotInForSalesSide", newName: "SttConstPosInvoiceTypeNotInForSalesSides");
        }
    }
}
