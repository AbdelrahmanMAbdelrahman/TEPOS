namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateConversionFactor : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PosUomGroupDetails", "ConversionFactor", c => c.Decimal(nullable: false, precision: 18, scale: 3));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PosUomGroupDetails", "ConversionFactor", c => c.Int(nullable: false));
        }
    }
}
