namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePosBillingTemplite33 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosBillprintTemplate", "TemplateType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosBillprintTemplate", "TemplateType");
        }
    }
}
