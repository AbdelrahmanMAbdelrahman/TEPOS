namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedSalesInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesInvoice", "TotalAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2,defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosSalesInvoice", "TotalAmt");
        }
    }
}
