namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posBillingrelatedTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosCityOrNearestZone",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .Index(t => new { t.Name, t.CmnCompanyId }, unique: true, name: "UK_PosCityOrNearestZone_Name_CmnCompanyId");
            
            CreateTable(
                "dbo.PosCustomers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerNo = c.Long(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Phone = c.String(),
                        AdditionalPhone = c.String(),
                        Address = c.String(),
                        Address2 = c.String(),
                        PosRegionId = c.Int(nullable: false),
                        PosCityOrNearestZoneId = c.Int(nullable: false),
                        IsPointAllowable = c.Boolean(nullable: false),
                        IsDueAllowable = c.Boolean(nullable: false),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.PosCityOrNearestZone", t => t.PosCityOrNearestZoneId, cascadeDelete: false)
                .ForeignKey("dbo.PosRegion", t => t.PosRegionId, cascadeDelete: false)
                .Index(t => new { t.CustomerNo, t.CmnCompanyId }, unique: true, name: "UK_PosCustomer_CustomerNo_CmnCompanyId")
                .Index(t => t.PosRegionId)
                .Index(t => t.PosCityOrNearestZoneId);
            
            CreateTable(
                "dbo.PosRegion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .Index(t => new { t.Name, t.CmnCompanyId }, unique: true, name: "UK_PosRegion_Name_CmnCompanyId");
            
            CreateTable(
                "dbo.PosSalesInvoice",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvDate = c.DateTime(nullable: false),
                        InvoiceNumber = c.Long(nullable: false),
                        PosCustomerId = c.Long(nullable: false),
                        PaidableAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DueAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.PosCustomers", t => t.PosCustomerId, cascadeDelete: false)
                .Index(t => new { t.InvoiceNumber, t.CmnCompanyId }, unique: true, name: "UK_PosSalesInvoice_InvoiceNumber_CmnCompanyId")
                .Index(t => t.PosCustomerId);
            
            CreateTable(
                "dbo.PosSalesInvoiceFreeProducts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosProductId = c.Long(nullable: false),
                        PosProductBatchId = c.Long(nullable: false),
                        Qty = c.Int(nullable: false),
                        PosSalesInvoiceId = c.Long(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PosSalesInvoice", t => t.PosSalesInvoiceId, cascadeDelete: false)
                .Index(t => t.PosSalesInvoiceId);
            
            CreateTable(
                "dbo.PosSalesInvoiceProducts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosProductId = c.Long(nullable: false),
                        PosProductBatchId = c.Long(nullable: false),
                        Qty = c.Int(nullable: false),
                        Vat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SD = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SchDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PosSalesInvoiceId = c.Long(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PosSalesInvoice", t => t.PosSalesInvoiceId, cascadeDelete: false)
                .Index(t => t.PosSalesInvoiceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosSalesInvoiceProducts", "PosSalesInvoiceId", "dbo.PosSalesInvoice");
            DropForeignKey("dbo.PosSalesInvoiceFreeProducts", "PosSalesInvoiceId", "dbo.PosSalesInvoice");
            DropForeignKey("dbo.PosSalesInvoice", "PosCustomerId", "dbo.PosCustomers");
            DropForeignKey("dbo.PosSalesInvoice", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosCustomers", "PosRegionId", "dbo.PosRegion");
            DropForeignKey("dbo.PosRegion", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosCustomers", "PosCityOrNearestZoneId", "dbo.PosCityOrNearestZone");
            DropForeignKey("dbo.PosCustomers", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosCityOrNearestZone", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosSalesInvoiceProducts", new[] { "PosSalesInvoiceId" });
            DropIndex("dbo.PosSalesInvoiceFreeProducts", new[] { "PosSalesInvoiceId" });
            DropIndex("dbo.PosSalesInvoice", new[] { "PosCustomerId" });
            DropIndex("dbo.PosSalesInvoice", "UK_PosSalesInvoice_InvoiceNumber_CmnCompanyId");
            DropIndex("dbo.PosRegion", "UK_PosRegion_Name_CmnCompanyId");
            DropIndex("dbo.PosCustomers", new[] { "PosCityOrNearestZoneId" });
            DropIndex("dbo.PosCustomers", new[] { "PosRegionId" });
            DropIndex("dbo.PosCustomers", "UK_PosCustomer_CustomerNo_CmnCompanyId");
            DropIndex("dbo.PosCityOrNearestZone", "UK_PosCityOrNearestZone_Name_CmnCompanyId");
            DropTable("dbo.PosSalesInvoiceProducts");
            DropTable("dbo.PosSalesInvoiceFreeProducts");
            DropTable("dbo.PosSalesInvoice");
            DropTable("dbo.PosRegion");
            DropTable("dbo.PosCustomers");
            DropTable("dbo.PosCityOrNearestZone");
        }
    }
}
