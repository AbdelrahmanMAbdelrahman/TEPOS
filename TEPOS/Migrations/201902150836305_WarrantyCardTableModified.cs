namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WarrantyCardTableModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WarrantyCards", "CmnCompanyId", c => c.Int(nullable: false));
            AddColumn("dbo.WarrantyCards", "CreatedBy", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WarrantyCards", "CreatedBy");
            DropColumn("dbo.WarrantyCards", "CmnCompanyId");
        }
    }
}
