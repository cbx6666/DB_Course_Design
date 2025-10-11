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

            // 索引配置
            builder.HasIndex(fo => fo.CartID).IsUnique();

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