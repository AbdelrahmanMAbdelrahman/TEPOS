namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateInvoice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosSalesInvoiceTenders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosSalesInvoiceId = c.Long(nullable: false),
                        PosTenderId = c.Int(nullable: false),
                        TenderAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PosSalesInvoice", t => t.PosSalesInvoiceId, cascadeDelete: false)
                .Index(t => t.PosSalesInvoiceId);
            
            AddColumn("dbo.PosSalesInvoice", "MrpTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PosSalesInvoice", "SdTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PosSalesInvoice", "VatOrTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PosSalesInvoice", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosSalesInvoiceTenders", "PosSalesInvoiceId", "dbo.PosSalesInvoice");
            DropIndex("dbo.PosSalesInvoiceTenders", new[] { "PosSalesInvoiceId" });
            DropColumn("dbo.PosSalesInvoice", "Discount");
            DropColumn("dbo.PosSalesInvoice", "VatOrTax");
            DropColumn("dbo.PosSalesInvoice", "SdTotal");
            DropColumn("dbo.PosSalesInvoice", "MrpTotal");
            DropTable("dbo.PosSalesInvoiceTenders");
        }
    }
}
