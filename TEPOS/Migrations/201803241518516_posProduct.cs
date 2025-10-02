namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posProduct : DbMigration
    {
        public override void Up()
        {
           
            
            CreateTable(
                "dbo.PosProducts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 1000),
                        BarCode = c.String(maxLength: 1000),
                        Image = c.Binary(),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId,false, "FK_PosProduct_CmnCompanyId")
                .Index(t => t.CmnCompanyId);
            
            
            
        }

        public override void Down()
        {
            DropTable("dbo.PosProducts");
        }
    }
}
