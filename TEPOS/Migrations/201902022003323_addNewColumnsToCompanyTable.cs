namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewColumnsToCompanyTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CmnCompanies", "IsSalesPriceIncludingVat", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CmnCompanies", "IsSalesPriceIncludingVat");
        }
    }
}
