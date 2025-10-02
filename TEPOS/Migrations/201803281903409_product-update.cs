namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PosProducts", "NameInOtherLang", c => c.String(nullable: false, maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PosProducts", "NameInOtherLang");
        }
    }
}
