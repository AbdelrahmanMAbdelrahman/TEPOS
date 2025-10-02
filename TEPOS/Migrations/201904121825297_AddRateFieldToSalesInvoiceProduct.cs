namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRateFieldToSalesInvoiceProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesInvoiceProducts", "Rate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosSalesInvoiceProducts", "Rate");
        }
    }
}
