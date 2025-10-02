namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateItemBarcode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosProductBatch", "BarCode", c => c.String(maxLength: 1000));
            DropColumn("dbo.PosProducts", "BarCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosProducts", "BarCode", c => c.String(maxLength: 1000));
            DropColumn("dbo.PosProductBatch", "BarCode");
        }
    }
}
