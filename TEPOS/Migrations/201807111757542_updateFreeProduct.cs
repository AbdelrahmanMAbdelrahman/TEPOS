namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateFreeProduct : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PosSalesInvoiceFreeProducts", "CreatedBy");
            DropColumn("dbo.PosSalesInvoiceFreeProducts", "CreatedDate");
            DropColumn("dbo.PosSalesInvoiceFreeProducts", "ModifiedBy");
            DropColumn("dbo.PosSalesInvoiceFreeProducts", "ModifideDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosSalesInvoiceFreeProducts", "ModifideDate", c => c.DateTime());
            AddColumn("dbo.PosSalesInvoiceFreeProducts", "ModifiedBy", c => c.Int());
            AddColumn("dbo.PosSalesInvoiceFreeProducts", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.PosSalesInvoiceFreeProducts", "CreatedBy", c => c.Int());
        }
    }
}
