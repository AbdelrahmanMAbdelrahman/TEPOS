namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTender2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PosTenders", "UK_PosTenders_Name_ConstraintType_CmnCompanyId");
            AddColumn("dbo.PosTenders", "Type", c => c.String(maxLength: 2));
            AddColumn("dbo.PosTenders", "IsEditable", c => c.Boolean(nullable: false,defaultValue:true));
            AddColumn("dbo.PosTenderTypes", "IsEditable", c => c.Boolean(nullable: false,defaultValue:true));
            CreateIndex("dbo.PosTenders", new[] { "Name", "Type", "CmnCompanyId" }, unique: true, name: "UK_PosTenders_Name_Type_CmnCompanyId");
            DropColumn("dbo.PosTenders", "ConstraintType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosTenders", "ConstraintType", c => c.String(maxLength: 2));
            DropIndex("dbo.PosTenders", "UK_PosTenders_Name_Type_CmnCompanyId");
            DropColumn("dbo.PosTenderTypes", "IsEditable");
            DropColumn("dbo.PosTenders", "IsEditable");
            DropColumn("dbo.PosTenders", "Type");
            CreateIndex("dbo.PosTenders", new[] { "Name", "ConstraintType", "CmnCompanyId" }, unique: true, name: "UK_PosTenders_Name_ConstraintType_CmnCompanyId");
        }
    }
}
