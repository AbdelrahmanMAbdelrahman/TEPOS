namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateInvoiceFreeProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesInvoiceFreeProducts", "ManualQty", c => c.Int(nullable: false,defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosSalesInvoiceFreeProducts", "ManualQty");
        }
    }
}
