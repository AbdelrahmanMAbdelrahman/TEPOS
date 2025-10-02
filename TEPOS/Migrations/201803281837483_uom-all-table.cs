namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uomalltable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PosUomGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                        CmnCompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .Index(t => t.CmnCompanyId);
            
            CreateTable(
                "dbo.PosUomGroupDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PosUomGroupId = c.Int(nullable: false),
                        PosUomMasterId = c.Int(nullable: false),
                        ConversionFactor = c.Int(nullable: false),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .ForeignKey("dbo.PosUomGroup", t => t.PosUomGroupId, cascadeDelete: false)
                .ForeignKey("dbo.PosUomMaster", t => t.PosUomMasterId, cascadeDelete: false)
                .Index(t => new { t.PosUomGroupId, t.PosUomMasterId, t.CmnCompanyId }, unique: true, name: "UK_PosUomGroupId_PosUomMaster_CmnCmpanyId");
            
            CreateTable(
                "dbo.PosUomMaster",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UomCode = c.String(nullable: false, maxLength: 50),
                        UomDescription = c.String(nullable: false, maxLength: 500),
                        CmnCompanyId = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifideDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CmnCompanies", t => t.CmnCompanyId, cascadeDelete: false)
                .Index(t => t.CmnCompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosUomGroupDetails", "PosUomMasterId", "dbo.PosUomMaster");
            DropForeignKey("dbo.PosUomMaster", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosUomGroupDetails", "PosUomGroupId", "dbo.PosUomGroup");
            DropForeignKey("dbo.PosUomGroupDetails", "CmnCompanyId", "dbo.CmnCompanies");
            DropForeignKey("dbo.PosUomGroup", "CmnCompanyId", "dbo.CmnCompanies");
            DropIndex("dbo.PosUomMaster", new[] { "CmnCompanyId" });
            DropIndex("dbo.PosUomGroupDetails", "UK_PosUomGroupId_PosUomMaster_CmnCmpanyId");
            DropIndex("dbo.PosUomGroup", new[] { "CmnCompanyId" });
            DropTable("dbo.PosUomMaster");
            DropTable("dbo.PosUomGroupDetails");
            DropTable("dbo.PosUomGroup");
        }
    }
}
