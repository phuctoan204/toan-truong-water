namespace WebBanHangOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedbv1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderHistories", "OrderId", "dbo.tb_Order");
            DropForeignKey("dbo.tb_Product", "ProductTypeId", "dbo.tb_ProductType");
            DropIndex("dbo.tb_Product", new[] { "ProductTypeId" });
            DropIndex("dbo.OrderHistories", new[] { "OrderId" });
            CreateTable(
                "dbo.LuckyDraws",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Prize = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.tb_Adv", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Adv", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Category", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Category", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_News", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_News", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Posts", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Posts", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Contact", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Contact", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Order", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Order", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Product", "ProductTypeId", c => c.Int());
            AlterColumn("dbo.tb_Product", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_Product", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ProductCategory", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.tb_ProductCategory", "ModifiedDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.tb_Product", "ProductTypeId");
            AddForeignKey("dbo.tb_Product", "ProductTypeId", "dbo.tb_ProductType", "Id");
            DropColumn("dbo.DiscountCodes", "UsageLimit");
            DropColumn("dbo.DiscountCodes", "UsedCount");
            DropColumn("dbo.DiscountCodes", "MinimumOrderAmount");
            DropColumn("dbo.DiscountCodes", "CreatedDate");
            DropColumn("dbo.DiscountCodes", "ModifiedDate");
            DropColumn("dbo.DiscountCodes", "CreatedBy");
            DropColumn("dbo.DiscountCodes", "ModifiedBy");
            DropColumn("dbo.OrderHistories", "CreatedDate");
            DropColumn("dbo.OrderHistories", "TotalAmount");
            DropColumn("dbo.OrderHistories", "Status");
            DropColumn("dbo.OrderHistories", "OrderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderHistories", "OrderId", c => c.Int(nullable: false));
            AddColumn("dbo.OrderHistories", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.OrderHistories", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderHistories", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.DiscountCodes", "ModifiedBy", c => c.String());
            AddColumn("dbo.DiscountCodes", "CreatedBy", c => c.String());
            AddColumn("dbo.DiscountCodes", "ModifiedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.DiscountCodes", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.DiscountCodes", "MinimumOrderAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.DiscountCodes", "UsedCount", c => c.Int(nullable: false));
            AddColumn("dbo.DiscountCodes", "UsageLimit", c => c.Int(nullable: false));
            DropForeignKey("dbo.tb_Product", "ProductTypeId", "dbo.tb_ProductType");
            DropIndex("dbo.tb_Product", new[] { "ProductTypeId" });
            AlterColumn("dbo.tb_ProductCategory", "ModifiedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_ProductCategory", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Product", "ModifiedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Product", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Product", "ProductTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.tb_Order", "ModifiedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Order", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Contact", "ModifiedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Contact", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Posts", "ModifiedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Posts", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_News", "ModifiedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_News", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Category", "ModifiedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Category", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Adv", "ModifiedDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.tb_Adv", "CreatedDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            DropTable("dbo.LuckyDraws");
            CreateIndex("dbo.OrderHistories", "OrderId");
            CreateIndex("dbo.tb_Product", "ProductTypeId");
            AddForeignKey("dbo.tb_Product", "ProductTypeId", "dbo.tb_ProductType", "Id", cascadeDelete: true);
            AddForeignKey("dbo.OrderHistories", "OrderId", "dbo.tb_Order", "Id", cascadeDelete: true);
        }
    }
}
