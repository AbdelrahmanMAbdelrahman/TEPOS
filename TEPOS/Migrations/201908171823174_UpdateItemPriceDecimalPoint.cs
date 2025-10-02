namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemPriceDecimalPoint : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.WarrantyCards");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.WarrantyCards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WarrantyNo = c.String(),
                        PurchaseDate = c.DateTime(nullable: false),
                        InvoiceNo = c.Long(nullable: false),
                        Branch = c.String(),
                        CustomerName = c.String(),
                        ContactNo = c.String(),
                        Model = c.String(),
                        ImeiNo = c.String(),
                        SerialNo = c.String(),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
