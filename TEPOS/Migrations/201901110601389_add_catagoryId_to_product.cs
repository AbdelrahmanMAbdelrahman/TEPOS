namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_catagoryId_to_product : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosProducts", "PosCategoryId", c => c.Int(nullable: false,defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosProducts", "PosCategoryId");
        }
    }
}
