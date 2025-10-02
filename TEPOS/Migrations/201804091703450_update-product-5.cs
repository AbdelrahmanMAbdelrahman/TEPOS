namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateproduct5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosProducts", "Vat", c => c.Decimal(precision: 18, scale: 2,nullable:true,defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosProducts", "Vat");
        }
    }
}
