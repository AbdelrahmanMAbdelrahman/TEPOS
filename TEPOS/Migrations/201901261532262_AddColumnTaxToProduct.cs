namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnTaxToProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosProducts", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PosProducts", "Vat", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PosProducts", "Vat", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.PosProducts", "Tax");
        }
    }
}
