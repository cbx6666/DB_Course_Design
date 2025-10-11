using BackEnd.Models;
using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 购物车实体配置
    /// </summary>
    public class ShoppingCartConfig : IEntityTypeConfiguration<ShoppingCart>
    {
        /// <summary>
        /// 配置购物车实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.ToTable("SHOPPING_CARTS");

            // 主键配置
            builder.HasKey(sc => sc.CartID);
            builder.Property(sc => sc.CartID).HasColumnName("CARTID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(sc => sc.LastUpdatedTime).HasColumnName("LASTUPDATEDTIME").IsRequired();
            builder.Property(sc => sc.TotalPrice)
                .HasColumnName("TOTALPRICE")
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0.00m);

            // 状态配置
            builder.Property(sc => sc.ShoppingCartState)
                .HasColumnName("SHOPPINGCARTSTATE")
                .IsRequired(false)
                .HasConversion<string>()
                .HasMaxLength(20);

            // 外键配置
            builder.Property(sc => sc.CustomerID).HasColumnName("CUSTOMERID").IsRequired();
            builder.Property(sc => sc.StoreID).HasColumnName("STOREID");

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<ShoppingCart> builder)
        {
            // 配置与Customer的多对一关系
            builder.HasOne(sc => sc.Customer)
                .WithMany(cu => cu.ShoppingCarts)
                .HasForeignKey(sc => sc.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Store的多对一关系
            builder.HasOne(sc => sc.Store)
                .WithMany(s => s.ShoppingCarts)
                .HasForeignKey(sc => sc.StoreID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}