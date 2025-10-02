namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedSalesInvoice1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesInvoice", "PosInvoiceType", c => c.Int(nullable: false,defaultValue:1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosSalesInvoice", "PosInvoiceType");
        }
    }
}
