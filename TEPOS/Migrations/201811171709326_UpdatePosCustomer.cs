namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePosCustomer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosCustomers", "IsPosBranchCustomer", c => c.Boolean(nullable: false,defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosCustomers", "IsPosBranchCustomer");
        }
    }
}
