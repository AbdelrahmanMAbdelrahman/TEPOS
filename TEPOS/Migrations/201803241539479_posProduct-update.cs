namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posProductupdate : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosProducts", new[] { "CmnCompanyId" });
            AddColumn("dbo.PosProducts", "Code", c => c.String(nullable: false, maxLength: 250));
            CreateIndex("dbo.PosProducts", new[] { "Code", "Name", "CmnCompanyId" }, unique: true, name: "UK_PosProduct_Code_Name_CmnCompanyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PosProducts", "UK_PosProduct_Code_Name_CmnCompanyId");
            DropColumn("dbo.PosProducts", "Code");
            CreateIndex("dbo.PosProducts", "CmnCompanyId");
        }
    }
}
