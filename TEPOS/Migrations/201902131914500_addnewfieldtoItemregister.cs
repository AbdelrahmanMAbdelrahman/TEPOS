namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewfieldtoItemregister : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosProducts", "IsHideToStock", c => c.Boolean(nullable: false,defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosProducts", "IsHideToStock");
        }
    }
}
