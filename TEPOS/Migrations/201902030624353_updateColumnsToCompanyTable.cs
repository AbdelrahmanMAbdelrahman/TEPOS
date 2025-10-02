namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateColumnsToCompanyTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CmnCompanies", "SalesPriceIncOrExcVat", c => c.Int(nullable: false,defaultValue:1));
            DropColumn("dbo.CmnCompanies", "IsSalesPriceIncludingVat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CmnCompanies", "IsSalesPriceIncludingVat", c => c.Boolean(nullable: false));
            DropColumn("dbo.CmnCompanies", "SalesPriceIncOrExcVat");
        }
    }
}
