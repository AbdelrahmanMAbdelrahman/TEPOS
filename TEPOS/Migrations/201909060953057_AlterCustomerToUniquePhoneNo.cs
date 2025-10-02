namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterCustomerToUniquePhoneNo : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.PosCustomers", new[] { "Phone", "CmnCompanyId" }, unique: true, name: "UK_PosCustomer_Phone_CmnCompanyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PosCustomers", "UK_PosCustomer_Phone_CmnCompanyId");
        }
    }
}
