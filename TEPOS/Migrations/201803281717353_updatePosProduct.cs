namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePosProduct : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosProducts", "UK_PosProduct_Code_CustomCode_CmnCompanyId");
            AddColumn("dbo.PosProducts", "CompanyCode", c => c.String(nullable: false, maxLength: 250));
            AddColumn("dbo.PosProducts", "ModifideDate", c => c.DateTime());
            AlterColumn("dbo.PosProducts", "CreatedBy", c => c.Int());
            AlterColumn("dbo.PosProducts", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.PosProducts", "ModifiedBy", c => c.Int());
            CreateIndex("dbo.PosProducts", new[] { "Code", "CompanyCode", "CmnCompanyId" }, unique: true, name: "UK_PosProduct_Code_CompanyCode_CmnCompanyId");
            DropColumn("dbo.PosProducts", "CustomCode");
            DropColumn("dbo.PosProducts", "ModifiedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosProducts", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PosProducts", "CustomCode", c => c.String(nullable: false, maxLength: 250));
            DropIndex("dbo.PosProducts", "UK_PosProduct_Code_CompanyCode_CmnCompanyId");
            AlterColumn("dbo.PosProducts", "ModifiedBy", c => c.Int(nullable: false));
            AlterColumn("dbo.PosProducts", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PosProducts", "CreatedBy", c => c.Int(nullable: false));
            DropColumn("dbo.PosProducts", "ModifideDate");
            DropColumn("dbo.PosProducts", "CompanyCode");
            CreateIndex("dbo.PosProducts", new[] { "Code", "CustomCode", "CmnCompanyId" }, unique: true, name: "UK_PosProduct_Code_CustomCode_CmnCompanyId");
        }
    }
}
