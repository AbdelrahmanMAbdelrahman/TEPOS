namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatestock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosStockTypes", "IsBaseStock", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosStockTypes", "IsBaseStock");
        }
    }
}
