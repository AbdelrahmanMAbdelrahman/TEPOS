namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SecResources", "ParentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SecResources", "ParentId", c => c.Int(nullable: false));
        }
    }
}
