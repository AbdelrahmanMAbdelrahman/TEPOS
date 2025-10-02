namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class schemeUpdate2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PosSchemeCustomerClass", "PosCustomerClass_Id", "dbo.PosCustomerClass");
            DropIndex("dbo.PosSchemeCustomerClass", "UK_PosSchemeCustomerClass_PosCustomerClassId_PosSchemeId");
            DropIndex("dbo.PosSchemeCustomerClass", new[] { "PosCustomerClass_Id" });
            DropColumn("dbo.PosSchemeCustomerClass", "PosCustomerClassId");
            RenameColumn(table: "dbo.PosSchemeCustomerClass", name: "PosCustomerClass_Id", newName: "PosCustomerClassId");
            AlterColumn("dbo.PosSchemeCustomerClass", "PosCustomerClassId", c => c.Int(nullable: false));
            AlterColumn("dbo.PosSchemeCustomerClass", "PosCustomerClassId", c => c.Int(nullable: false));
            CreateIndex("dbo.PosSchemeCustomerClass", new[] { "PosSchemeId", "PosCustomerClassId" }, unique: true, name: "UK_PosSchemeCustomerClass_PosCustomerClassId_PosSchemeId");
            AddForeignKey("dbo.PosSchemeCustomerClass", "PosCustomerClassId", "dbo.PosCustomerClass", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosSchemeCustomerClass", "PosCustomerClassId", "dbo.PosCustomerClass");
            DropIndex("dbo.PosSchemeCustomerClass", "UK_PosSchemeCustomerClass_PosCustomerClassId_PosSchemeId");
            AlterColumn("dbo.PosSchemeCustomerClass", "PosCustomerClassId", c => c.Int());
            AlterColumn("dbo.PosSchemeCustomerClass", "PosCustomerClassId", c => c.Long(nullable: false));
            RenameColumn(table: "dbo.PosSchemeCustomerClass", name: "PosCustomerClassId", newName: "PosCustomerClass_Id");
            AddColumn("dbo.PosSchemeCustomerClass", "PosCustomerClassId", c => c.Long(nullable: false));
            CreateIndex("dbo.PosSchemeCustomerClass", "PosCustomerClass_Id");
            CreateIndex("dbo.PosSchemeCustomerClass", new[] { "PosSchemeId", "PosCustomerClassId" }, unique: true, name: "UK_PosSchemeCustomerClass_PosCustomerClassId_PosSchemeId");
            AddForeignKey("dbo.PosSchemeCustomerClass", "PosCustomerClass_Id", "dbo.PosCustomerClass", "Id");
        }
    }
}
