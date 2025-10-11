using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 售后申请实体配置
    /// </summary>
    public class AfterSaleApplicationConfig : IEntityTypeConfiguration<AfterSaleApplication>
    {
        /// <summary>
        /// 配置售后申请实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<AfterSaleApplication> builder)
        {
            builder.ToTable("AFTER_SALE_APPLICATIONS");

            // 主键配置
            builder.HasKey(asa => asa.ApplicationID);
            builder.Property(asa => asa.ApplicationID).HasColumnName("APPLICATIONID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(asa => asa.Description).HasColumnName("DESCRIPTION").IsRequired().HasMaxLength(255);
            builder.Property(asa => asa.ApplicationTime).HasColumnName("APPLICATIONTIME").IsRequired();

            // 状态配置
            builder.Property(asa => asa.AfterSaleState)
                .HasColumnName("AFTERSALESTATE")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(AfterSaleState.Pending);

            // 处理相关属性配置
            builder.Property(asa => asa.ProcessingResult).HasColumnName("PROCESSINGRESULT").IsRequired(false).HasMaxLength(255);
            builder.Property(asa => asa.ProcessingReason).HasColumnName("PROCESSINGREASON").IsRequired(false).HasMaxLength(255);
            builder.Property(asa => asa.ProcessingRemark).HasColumnName("PROCESSINGREMARK").IsRequired(false).HasMaxLength(255);

            // 外键配置
            builder.Property(asa => asa.OrderID).HasColumnName("ORDERID").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<AfterSaleApplication> builder)
        {
            // 配置与FoodOrder的多对一关系
            builder.HasOne(asa => asa.Order)
                .WithMany(fo => fo.AfterSaleApplications)
                .HasForeignKey(asa => asa.OrderID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}