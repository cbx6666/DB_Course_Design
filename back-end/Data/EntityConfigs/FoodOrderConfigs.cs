using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 订单实体配置
    /// </summary>
    public class FoodOrderConfig : IEntityTypeConfiguration<FoodOrder>
    {
        /// <summary>
        /// 配置订单实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<FoodOrder> builder)
        {
            builder.ToTable("FOOD_ORDERS");

            // 主键配置
            builder.HasKey(fo => fo.OrderID);
            builder.Property(fo => fo.OrderID).HasColumnName("ORDERID").ValueGeneratedOnAdd();

            // 时间相关属性配置
            builder.Property(fo => fo.OrderTime).HasColumnName("ORDERTIME").IsRequired();
            builder.Property(fo => fo.PaymentTime).HasColumnName("PAYMENTTIME").IsRequired(false);

            // 其他属性配置
            builder.Property(fo => fo.Remarks).HasColumnName("REMARKS").HasMaxLength(255);
            builder.Property(fo => fo.DeliveryFee).HasColumnName("DELIVERYFEE").IsRequired();

            // 状态配置
            builder.Property(fo => fo.FoodOrderState)
                .HasColumnName("FOODORDERSTATE")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            // 外键配置
            builder.Property(fo => fo.CustomerID).HasColumnName("CUSTOMERID").IsRequired();
            builder.Property(fo => fo.CartID).HasColumnName("CARTID").IsRequired(false);
            builder.Property(fo => fo.StoreID).HasColumnName("STOREID").IsRequired();
            builder.Property(fo => fo.DeliveryInfoID).HasColumnName("DELIVERYINFOID").IsRequired();

            // 索引配置
            builder.HasIndex(fo => fo.CartID).IsUnique();
            
            // 性能优化索引：根据查询模式添加常用字段索引
            // StoreID 索引：商家查询订单时使用
            builder.HasIndex(fo => fo.StoreID);
            
            // CustomerID 索引：消费者查询订单时使用
            builder.HasIndex(fo => fo.CustomerID);
            
            // 复合索引：商家按时间查询订单（StoreID + OrderTime）
            builder.HasIndex(fo => new { fo.StoreID, fo.OrderTime });
            
            // 复合索引：消费者按时间查询订单（CustomerID + OrderTime）
            builder.HasIndex(fo => new { fo.CustomerID, fo.OrderTime });
            
            // 复合索引：月销量统计查询（StoreID + FoodOrderState + PaymentTime）
            builder.HasIndex(fo => new { fo.StoreID, fo.FoodOrderState, fo.PaymentTime });

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<FoodOrder> builder)
        {
            // 配置与Customer的多对一关系
            builder.HasOne(fo => fo.Customer)
                .WithMany(c => c.FoodOrders)
                .HasForeignKey(fo => fo.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置与ShoppingCart的一对一关系
            builder.HasOne(fo => fo.Cart)
                .WithOne(c => c.Order)
                .HasForeignKey<FoodOrder>(fo => fo.CartID)
                .OnDelete(DeleteBehavior.SetNull);

            // 配置与Store的多对一关系
            builder.HasOne(fo => fo.Store)
                .WithMany(s => s.FoodOrders)
                .HasForeignKey(fo => fo.StoreID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}