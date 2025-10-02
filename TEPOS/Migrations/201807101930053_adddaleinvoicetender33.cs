namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddaleinvoicetender33 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PosSalesInvoice", "InvDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.PosScheme", "DateFrom", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.PosScheme", "DateTo", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PosScheme", "DateTo", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PosScheme", "DateFrom", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PosSalesInvoice", "InvDate", c => c.DateTime(nullable: false));
        }
    }
}
