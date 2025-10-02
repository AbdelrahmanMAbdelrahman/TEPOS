namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosProducts", "IsPriceChangeable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosProducts", "IsPriceChangeable");
        }
    }
}
