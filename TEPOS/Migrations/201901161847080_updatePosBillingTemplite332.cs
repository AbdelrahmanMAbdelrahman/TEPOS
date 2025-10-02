namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePosBillingTemplite332 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosBillprintTemplateOfBranch", "TemplateType", c => c.Int());
            DropColumn("dbo.PosBillprintTemplate", "TemplateType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PosBillprintTemplate", "TemplateType", c => c.Int());
            DropColumn("dbo.PosBillprintTemplateOfBranch", "TemplateType");
        }
    }
}
