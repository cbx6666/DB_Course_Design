using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 优惠券管理实体配置
    /// </summary>
    public class CouponManagerConfig : IEntityTypeConfiguration<CouponManager>
    {
        /// <summary>
        /// 配置优惠券管理实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<CouponManager> builder)
        {
            builder.ToTable("COUPON_MANAGERS");

            // 主键配置
            builder.HasKey(cm => cm.CouponManagerID);
            builder.Property(cm => cm.CouponManagerID)
                .HasColumnName("COUPONMANAGERID")
                .ValueGeneratedOnAdd();

            // 必填字段配置
            builder.Property(cm => cm.MinimumSpend)
                .HasColumnName("MINIMUMSPEND")
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            builder.Property(cm => cm.Value)
                .HasColumnName("VALUE")
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            builder.Property(cm => cm.ValidFrom).HasColumnName("VALIDFROM").IsRequired();
            builder.Property(cm => cm.ValidTo).HasColumnName("VALIDTO").IsRequired();
            builder.Property(cm => cm.StoreID).HasColumnName("STOREID").IsRequired();

            // 可空字段配置
            builder.Property(cm => cm.CouponName).HasColumnName("COUPONNAME").HasMaxLength(100);
            builder.Property(cm => cm.CouponType)
                .HasColumnName("COUPONTYPE")
                .HasConversion<string>()
                .HasMaxLength(100);
            builder.Property(cm => cm.TotalQuantity).HasColumnName("TOTALQUANTITY");
            builder.Property(cm => cm.UsedQuantity).HasColumnName("USEDQUANTITY");
            builder.Property(cm => cm.Description).HasColumnName("DESCRIPTION").HasMaxLength(500);

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<CouponManager> builder)
        {
            // 配置与Store的多对一关系
            builder.HasOne(cm => cm.Store)
                .WithMany(s => s.CouponManagers)
                .HasForeignKey(cm => cm.StoreID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
