namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateResource1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecResources", "IsReport", c => c.Boolean(nullable: false,defaultValue:false));
            AddColumn("dbo.SecResourcePermission", "IsReport", c => c.Boolean(nullable: false,defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecResourcePermission", "IsReport");
            DropColumn("dbo.SecResources", "IsReport");
        }
    }
}
