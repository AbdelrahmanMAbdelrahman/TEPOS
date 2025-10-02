namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewRemarksToSalInvoiceTender : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesInvoiceTenders", "Remarks", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosSalesInvoiceTenders", "Remarks");
        }
    }
}
