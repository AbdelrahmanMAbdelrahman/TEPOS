
namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class schemeUpdate1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosCustomerClass", "UK_PosCustomerClass_Name_CmnCompanyId");
            AlterColumn("dbo.PosCustomerClass", "Name", c => c.String(maxLength: 100));
            CreateIndex("dbo.PosCustomerClass", new[] { "Name", "CmnCompanyId" }, unique: true, name: "UK_PosCustomerClass_Name_CmnCompanyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PosCustomerClass", "UK_PosCustomerClass_Name_CmnCompanyId");
            AlterColumn("dbo.PosCustomerClass", "Name", c => c.String());
            CreateIndex("dbo.PosCustomerClass", new[] { "Name", "CmnCompanyId" }, unique: true, name: "UK_PosCustomerClass_Name_CmnCompanyId");
        }
    }
}
