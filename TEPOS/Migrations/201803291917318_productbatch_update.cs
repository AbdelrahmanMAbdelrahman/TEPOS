namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productbatch_update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosProductBatch", "Weight", c => c.Int(nullable: false,defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosProductBatch", "Weight");
        }
    }
}
