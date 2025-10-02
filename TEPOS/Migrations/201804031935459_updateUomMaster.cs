namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUomMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosUomMaster", "IsBaseUom", c => c.Boolean(nullable: false,defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosUomMaster", "IsBaseUom");
        }
    }
}
