using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 购物车项实体配置
    /// </summary>
    public class ShoppingCartItemConfig : IEntityTypeConfiguration<ShoppingCartItem>
    {
        /// <summary>
        /// 配置购物车项实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.ToTable("SHOPPING_CART_ITEMS");

            // 主键配置
            builder.HasKey(sci => sci.ItemID);
            builder.Property(sci => sci.ItemID).HasColumnName("ITEMID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(sci => sci.Quantity).HasColumnName("QUANTITY").IsRequired();
            builder.Property(sci => sci.TotalPrice)
                .HasColumnName("TOTALPRICE")
                .HasColumnType("decimal(10,2)");

            // 外键配置
            builder.Property(sci => sci.DishID).HasColumnName("DISHID").IsRequired();
            builder.Property(sci => sci.CartID).HasColumnName("CARTID").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            // 配置与Dish的多对一关系
            builder.HasOne(sci => sci.Dish)
                .WithMany(d => d.ShoppingCartItems)
                .HasForeignKey(sci => sci.DishID)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置与ShoppingCart的多对一关系
            builder.HasOne(sci => sci.Cart)
                .WithMany(sc => sc.ShoppingCartItems)
                .HasForeignKey(sci => sci.CartID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
