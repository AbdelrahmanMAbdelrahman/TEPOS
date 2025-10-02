namespace ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStockModel : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.PosProducts", "PosProductCategoryId", "dbo.PosProductCategories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PosProducts", "PosProductCategoryId", "dbo.PosProductCategories");
        }
    }
}
