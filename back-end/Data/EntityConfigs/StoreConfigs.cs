using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 店铺实体配置
    /// </summary>
    public class StoreConfig : IEntityTypeConfiguration<Store>
    {
        /// <summary>
        /// 配置店铺实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("STORES");

            // 主键配置
            builder.HasKey(s => s.StoreID);
            builder.Property(s => s.StoreID).HasColumnName("STOREID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(s => s.StoreName).HasColumnName("STORENAME").IsRequired().HasMaxLength(50);
            builder.Property(s => s.StoreAddress).HasColumnName("STOREADDRESS").IsRequired().HasMaxLength(100);
            builder.Property(s => s.StoreFeatures).HasColumnName("STOREFEATURES").IsRequired(false).HasMaxLength(500);
            builder.Property(s => s.StoreImage).HasColumnName("STOREIMAGE").HasMaxLength(500).IsRequired(false);

            // 位置信息配置
            builder.Property(s => s.Latitude)
                .HasColumnName("LATITUDE")
                .HasColumnType("decimal(10,6)")
                .IsRequired(false);

            builder.Property(s => s.Longitude)
                .HasColumnName("LONGITUDE")
                .HasColumnType("decimal(10,6)")
                .IsRequired(false);

            // 营业时间配置
            builder.Property(s => s.OpenTime).HasColumnName("OPENTIME").IsRequired();
            builder.Property(s => s.CloseTime).HasColumnName("CLOSETIME").IsRequired();

            // 统计信息配置
            builder.Property(s => s.AverageRating)
                .HasColumnName("AVERAGERATING")
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0.00m);

            builder.Property(s => s.MonthlySales).HasColumnName("MONTHLYSALES").IsRequired();
            builder.Property(s => s.StoreCreationTime).HasColumnName("STORECREATIONTIME").IsRequired();

            // 状态和分类配置
            builder.Property(s => s.StoreState)
                .HasColumnName("STORESTATE")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(s => s.StoreCategory)
                .HasColumnName("STORECATEGORY")
                .IsRequired()
                .HasMaxLength(20);

            // 外键配置
            builder.Property(s => s.SellerID).HasColumnName("SELLERID").IsRequired();

            // 忽略计算属性
            builder.Ignore(s => s.IsOpen);
            builder.Ignore(s => s.BusinessHoursDisplay);

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Store> builder)
        {
            // 配置与Seller的一对一关系
            builder.HasOne(s => s.Seller)
                .WithOne(seller => seller.Store)
                .HasForeignKey<Store>(s => s.SellerID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}