namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PosProductBatch_1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PosProductBatch", "DateFrom", c => c.DateTime(storeType: "date"));
            AlterColumn("dbo.PosProductBatch", "DateTo", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PosProductBatch", "DateTo", c => c.DateTime());
            AlterColumn("dbo.PosProductBatch", "DateFrom", c => c.DateTime());
        }
    }
}
