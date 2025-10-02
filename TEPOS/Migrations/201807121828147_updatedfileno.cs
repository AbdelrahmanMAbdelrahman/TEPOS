namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedfileno : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId", c => c.Long());
            CreateIndex("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId");
            AddForeignKey("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId", "dbo.PosInvoiceGlobalFileNumber", "Id");
            DropColumn("dbo.PosInvoiceGlobalFileNumber", "CreatedBy");
            DropColumn("dbo.PosInvoiceGlobalFileNumber", "CreatedDate");
            DropColumn("dbo.PosInvoiceGlobalFileNumber", "ModifiedBy");
            DropColumn("dbo.PosInvoiceGlobalFileNumber", "ModifideDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosInvoiceGlobalFileNumber", "ModifideDate", c => c.DateTime());
            AddColumn("dbo.PosInvoiceGlobalFileNumber", "ModifiedBy", c => c.Int());
            AddColumn("dbo.PosInvoiceGlobalFileNumber", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.PosInvoiceGlobalFileNumber", "CreatedBy", c => c.Int());
            DropForeignKey("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId", "dbo.PosInvoiceGlobalFileNumber");
            DropIndex("dbo.PosSalesInvoice", new[] { "PosInvoiceGlobalFileNumberId" });
            DropColumn("dbo.PosSalesInvoice", "PosInvoiceGlobalFileNumberId");
        }
    }
}
