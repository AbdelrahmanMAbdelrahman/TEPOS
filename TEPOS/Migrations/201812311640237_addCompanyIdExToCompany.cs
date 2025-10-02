namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCompanyIdExToCompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CmnCompanies", "CompanyIdOnExs", c => c.Int(defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CmnCompanies", "CompanyIdOnExs");
        }
    }
}
