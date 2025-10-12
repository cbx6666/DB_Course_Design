using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BackEnd.Models;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 收货信息实体配置
    /// </summary>
    public class DeliveryInfoConfig : IEntityTypeConfiguration<DeliveryInfo>
    {
        /// <summary>
        /// 配置收货信息实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<DeliveryInfo> builder)
        {
            builder.ToTable("DELIVERY_INFOS");

            // 主键配置
            builder.HasKey(di => di.DeliveryInfoID);
            builder.Property(di => di.DeliveryInfoID).HasColumnName("DELIVERYINFOID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(di => di.Address).HasColumnName("ADDRESS").IsRequired().HasMaxLength(200);
            builder.Property(di => di.PhoneNumber).HasColumnName("PHONENUMBER").IsRequired().HasMaxLength(20);
            builder.Property(di => di.Name).HasColumnName("NAME").IsRequired().HasMaxLength(50);
            builder.Property(di => di.Gender).HasColumnName("GENDER").HasMaxLength(10).IsRequired(false);
            builder.Property(di => di.IsDefault).HasColumnName("ISDEFAULT").IsRequired().HasDefaultValue(false);
            builder.Property(di => di.CustomerID).HasColumnName("CUSTOMERID").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<DeliveryInfo> builder)
        {
            // 配置与Customer的多对一关系
            builder.HasOne(di => di.Customer)
                .WithMany(c => c.DeliveryInfos)
                .HasForeignKey(di => di.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
