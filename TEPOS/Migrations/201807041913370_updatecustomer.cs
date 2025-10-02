namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecustomer : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PosCustomers", "FirstName", c => c.String(maxLength: 150));
            AlterColumn("dbo.PosCustomers", "LastName", c => c.String(maxLength: 150));
            AlterColumn("dbo.PosCustomers", "Phone", c => c.String(maxLength: 20));
            AlterColumn("dbo.PosCustomers", "AdditionalPhone", c => c.String(maxLength: 20));
            AlterColumn("dbo.PosCustomers", "Address", c => c.String(maxLength: 250));
            AlterColumn("dbo.PosCustomers", "Address2", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PosCustomers", "Address2", c => c.String());
            AlterColumn("dbo.PosCustomers", "Address", c => c.String());
            AlterColumn("dbo.PosCustomers", "AdditionalPhone", c => c.String());
            AlterColumn("dbo.PosCustomers", "Phone", c => c.String());
            AlterColumn("dbo.PosCustomers", "LastName", c => c.String());
            AlterColumn("dbo.PosCustomers", "FirstName", c => c.String());
        }
    }
}
