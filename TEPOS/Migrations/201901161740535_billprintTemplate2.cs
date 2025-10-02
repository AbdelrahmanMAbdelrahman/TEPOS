namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class billprintTemplate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosBillprintTemplate", "Method", c => c.String(maxLength: 250));
            AlterColumn("dbo.PosBillprintTemplate", "Name", c => c.String(maxLength: 250));
            AlterColumn("dbo.PosBillprintTemplate", "Remarks", c => c.String(maxLength: 500));
            AlterColumn("dbo.PosBillprintTemplateOfBranch", "Remarks", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PosBillprintTemplateOfBranch", "Remarks", c => c.Int(nullable: false));
            AlterColumn("dbo.PosBillprintTemplate", "Remarks", c => c.String());
            AlterColumn("dbo.PosBillprintTemplate", "Name", c => c.String());
            DropColumn("dbo.PosBillprintTemplate", "Method");
        }
    }
}
