namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewColumntoSalesInvoice : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SecModules", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosBillingReportTexts", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosBranch", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosCityOrNearestZone", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosCustomerClass", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosCustomers", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosRegion", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosInvoiceGlobalFileNumber", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosProductBatch", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosProducts", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosSalesInvoice", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosTenders", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosTenderTypes", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosStocks", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosSuppliers", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosStockTypes", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosUomGroup", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosUomGroupDetails", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosUomMaster", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosScheme", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.SecUsers", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.SecUserBranch", "CmnCompanyId", "dbo.CmnCompanies");
            AddColumn("dbo.PosSalesInvoice", "IsReceiveTransferStock", c => c.Boolean(nullable: false,defaultValue:false));
            AlterColumn("dbo.CmnCompanies", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.CmnCompanies", "Email", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.CmnCompanies", "Phone", c => c.String(nullable: false, maxLength: 50));
            AddForeignKey("dbo.SecModules", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosBillingReportTexts", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosBranch", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosCityOrNearestZone", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosCustomerClass", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosCustomers", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosRegion", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosInvoiceGlobalFileNumber", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosProductBatch", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosProducts", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosSalesInvoice", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosTenders", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosTenderTypes", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosStocks", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosSuppliers", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosStockTypes", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosUomGroup", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosUomGroupDetails", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosUomMaster", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosScheme", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.SecUsers", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.SecUserBranch", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SecUserBranch", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.SecUsers", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosScheme", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosUomMaster", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosUomGroupDetails", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosUomGroup", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosStockTypes", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosSuppliers", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosStocks", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosTenderTypes", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosTenders", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosSalesInvoice", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosProducts", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosProductBatch", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosInvoiceGlobalFileNumber", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosRegion", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosCustomers", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosCustomerClass", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosCityOrNearestZone", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosBranch", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosBillingReportTexts", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.SecModules", "CmnCompanyId", "dbo.CmnCompanies");
           AlterColumn("dbo.CmnCompanies", "Phone", c => c.String(maxLength: 50));
            AlterColumn("dbo.CmnCompanies", "Email", c => c.String(maxLength: 50));
            AlterColumn("dbo.CmnCompanies", "Id", c => c.Int(nullable: false, identity: false));
            DropColumn("dbo.PosSalesInvoice", "IsReceiveTransferStock");
            
            AddForeignKey("dbo.SecUserBranch", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.SecUsers", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosScheme", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosUomMaster", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosUomGroupDetails", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosUomGroup", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosStockTypes", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosSuppliers", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosStocks", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosTenderTypes", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosTenders", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosSalesInvoice", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosProducts", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosProductBatch", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosInvoiceGlobalFileNumber", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosRegion", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosCustomers", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosCustomerClass", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosCityOrNearestZone", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosBranch", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PosBillingReportTexts", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.SecModules", "CmnCompanyId", "dbo.CmnCompanies", "Id", cascadeDelete: false);
        }
    }
}
