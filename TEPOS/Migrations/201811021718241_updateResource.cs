namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateResource : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SecResourcePermission", "SecResourceId", "dbo.SecResources");
            DropIndex("dbo.SecResourcePermission", new[] { "SecResourceId" });
            AddColumn("dbo.SecResources", "RoleLevel", c => c.Int(nullable: false));
            AddColumn("dbo.SecResourcePermission", "RoleLevel", c => c.Int(nullable: false));
            AlterColumn("dbo.SecResources", "Serial", c => c.Int(nullable: false));
            AlterColumn("dbo.SecResources", "Level", c => c.Int(nullable: false));
            AlterColumn("dbo.SecResourcePermission", "SecResourceId", c => c.Int(nullable: false));
            AlterColumn("dbo.SecResourcePermission", "Serial", c => c.Int(nullable: false));
            AlterColumn("dbo.SecResourcePermission", "Level", c => c.Int(nullable: false));
            AlterColumn("dbo.SecRoles", "Level", c => c.Int(nullable: false));
            CreateIndex("dbo.SecResourcePermission", "SecResourceId");
            AddForeignKey("dbo.SecResourcePermission", "SecResourceId", "dbo.SecResources", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SecResourcePermission", "SecResourceId", "dbo.SecResources");
            DropIndex("dbo.SecResourcePermission", new[] { "SecResourceId" });
            AlterColumn("dbo.SecRoles", "Level", c => c.Int());
            AlterColumn("dbo.SecResourcePermission", "Level", c => c.Int());
            AlterColumn("dbo.SecResourcePermission", "Serial", c => c.Int());
            AlterColumn("dbo.SecResourcePermission", "SecResourceId", c => c.Int());
            AlterColumn("dbo.SecResources", "Level", c => c.Int());
            AlterColumn("dbo.SecResources", "Serial", c => c.Int());
            CreateIndex("dbo.SecResourcePermission", "SecResourceId");
            AddForeignKey("dbo.SecResourcePermission", "SecResourceId", "dbo.SecResources", "Id");
        }
    }
}
