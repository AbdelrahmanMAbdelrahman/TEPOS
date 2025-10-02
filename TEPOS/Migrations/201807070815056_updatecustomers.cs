namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecustomers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosCustomers", "IsDefaultPosCustomer", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosCustomers", "IsDefaultPosCustomer");
        }
    }
}
