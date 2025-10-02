namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class last : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosCustomers", "PosCustomerClassId", c => c.Int(nullable: false));
            CreateIndex("dbo.PosCustomers", "PosCustomerClassId");
            AddForeignKey("dbo.PosCustomers", "PosCustomerClassId", "dbo.PosCustomerClass", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosCustomers", "PosCustomerClassId", "dbo.PosCustomerClass");
            DropIndex("dbo.PosCustomers", new[] { "PosCustomerClassId" });
            DropColumn("dbo.PosCustomers", "PosCustomerClassId");
        }
    }
}
