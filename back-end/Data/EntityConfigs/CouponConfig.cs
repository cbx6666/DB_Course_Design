using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 优惠券实体配置
    /// </summary>
    public class CouponConfig : IEntityTypeConfiguration<Coupon>
    {
        /// <summary>
        /// 配置优惠券实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("COUPONS");

            // 主键配置
            builder.HasKey(c => c.CouponID);
            builder.Property(c => c.CouponID).HasColumnName("COUPONID").ValueGeneratedOnAdd();

            // 状态配置
            builder.Property(c => c.CouponState)
                .HasColumnName("COUPONSTATE")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // 外键配置
            builder.Property(c => c.CouponManagerID).HasColumnName("COUPONMANAGERID").IsRequired();
            builder.Property(c => c.CustomerID).HasColumnName("CUSTOMERID").IsRequired();
            builder.Property(c => c.OrderID).HasColumnName("ORDERID").IsRequired(false);

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Coupon> builder)
        {
            // 配置与CouponManager的多对一关系
            builder.HasOne(c => c.CouponManager)
                .WithMany(cm => cm.Coupons)
                .HasForeignKey(c => c.CouponManagerID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Customer的多对一关系
            builder.HasOne(c => c.Customer)
                .WithMany(cu => cu.Coupons)
                .HasForeignKey(c => c.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与FoodOrder的多对一关系
            builder.HasOne(c => c.Order)
                .WithMany(o => o.Coupons)
                .HasForeignKey(c => c.OrderID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}