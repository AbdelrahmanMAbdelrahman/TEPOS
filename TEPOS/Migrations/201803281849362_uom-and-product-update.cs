namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uomandproductupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosProducts", "PosUomGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.PosProducts", "PosUomGroupId");
            AddForeignKey("dbo.PosProducts", "PosUomGroupId", "dbo.PosUomGroup", "Id", cascadeDelete: false);
            DropColumn("dbo.PosUomGroupDetails", "CreatedBy");
            DropColumn("dbo.PosUomGroupDetails", "CreatedDate");
            DropColumn("dbo.PosUomGroupDetails", "ModifiedBy");
            DropColumn("dbo.PosUomGroupDetails", "ModifideDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosUomGroupDetails", "ModifideDate", c => c.DateTime());
            AddColumn("dbo.PosUomGroupDetails", "ModifiedBy", c => c.Int());
            AddColumn("dbo.PosUomGroupDetails", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.PosUomGroupDetails", "CreatedBy", c => c.Int());
            DropForeignKey("dbo.PosProducts", "PosUomGroupId", "dbo.PosUomGroup");
            DropIndex("dbo.PosProducts", new[] { "PosUomGroupId" });
            DropColumn("dbo.PosProducts", "PosUomGroupId");
        }
    }
}
