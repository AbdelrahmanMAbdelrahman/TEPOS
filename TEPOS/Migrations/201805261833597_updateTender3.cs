namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTender3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PosTenders", "PosTenderTypeId", "dbo.PosTenderTypes");
            DropIndex("dbo.PosTenders", new[] { "PosTenderTypeId" });
            AlterColumn("dbo.PosTenders", "PosTenderTypeId", c => c.Int());
            CreateIndex("dbo.PosTenders", "PosTenderTypeId");
            AddForeignKey("dbo.PosTenders", "PosTenderTypeId", "dbo.PosTenderTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosTenders", "PosTenderTypeId", "dbo.PosTenderTypes");
            DropIndex("dbo.PosTenders", new[] { "PosTenderTypeId" });
            AlterColumn("dbo.PosTenders", "PosTenderTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.PosTenders", "PosTenderTypeId");
            AddForeignKey("dbo.PosTenders", "PosTenderTypeId", "dbo.PosTenderTypes", "Id", cascadeDelete: true);
        }
    }
}
