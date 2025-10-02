namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddaleinvoicetender : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PosSalesInvoiceTenders", newName: "PosSalesInvoiceTenders");
            CreateIndex("dbo.PosSalesInvoiceTenders", "PosTenderId");
            AddForeignKey("dbo.PosSalesInvoiceTenders", "PosTenderId", "dbo.PosTenders", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosSalesInvoiceTenders", "PosTenderId", "dbo.PosTenders");
            DropIndex("dbo.PosSalesInvoiceTenders", new[] { "PosTenderId" });
        }
    }
}
