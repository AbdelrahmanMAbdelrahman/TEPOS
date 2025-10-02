namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTblSalesInvAndProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesInvoice", "OtherDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2,defaultValue:0));
            AddColumn("dbo.PosSalesInvoiceProducts", "OtherDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2,defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosSalesInvoiceProducts", "OtherDiscount");
            DropColumn("dbo.PosSalesInvoice", "OtherDiscount");
        }
    }
}
