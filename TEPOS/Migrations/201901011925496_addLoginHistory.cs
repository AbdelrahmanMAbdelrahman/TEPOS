namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addLoginHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SecLoginHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginDateTime = c.String(maxLength: 1000),
                        SecUserId = c.Int(nullable: false),
                        IpAddress = c.String(maxLength: 20),
                        LicenseKey = c.String(maxLength: 1000),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CmnUnitBreakDown",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        BigUnitName = c.String(nullable: false, maxLength: 20),
                        BigUnitValue = c.Int(nullable: false),
                        SmallUnitName = c.String(nullable: false, maxLength: 20),
                        SmallUnitValue = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.SecLoginHistories");
                 }
    }
}
