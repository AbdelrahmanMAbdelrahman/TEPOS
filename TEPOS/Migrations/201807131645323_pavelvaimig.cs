namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pavelvaimig : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosScheme", "UK_PosScheme_SchemeCode_CmnCompanyId");
            AlterColumn("dbo.PosScheme", "SchemeCode", c => c.Long(nullable: false));
            CreateIndex("dbo.PosScheme", new[] { "SchemeCode", "CmnCompanyId" }, unique: true, name: "UK_PosScheme_SchemeCode_CmnCompanyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PosScheme", "UK_PosScheme_SchemeCode_CmnCompanyId");
            AlterColumn("dbo.PosScheme", "SchemeCode", c => c.Long(nullable: false));
            CreateIndex("dbo.PosScheme", new[] { "SchemeCode", "CmnCompanyId" }, unique: true, name: "UK_PosScheme_SchemeCode_CmnCompanyId");
        }
    }
}
