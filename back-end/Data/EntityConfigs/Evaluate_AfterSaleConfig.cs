using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 售后申请评估实体配置
    /// </summary>
    public class Evaluate_AfterSaleConfig : IEntityTypeConfiguration<Evaluate_AfterSale>
    {
        /// <summary>
        /// 配置售后申请评估实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Evaluate_AfterSale> builder)
        {
            builder.ToTable("EVALUATE_AFTER_SALE");

            // 复合主键配置
            builder.HasKey(eas => new { eas.AdminID, eas.ApplicationID });

            // 属性配置
            builder.Property(eas => eas.AdminID).HasColumnName("ADMINID");
            builder.Property(eas => eas.ApplicationID).HasColumnName("APPLICATIONID");

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Evaluate_AfterSale> builder)
        {
            // 配置与Administrator的多对一关系
            builder.HasOne(eas => eas.Admin)
                .WithMany(a => a.EvaluateAfterSales)
                .HasForeignKey(eas => eas.AdminID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与AfterSaleApplication的多对一关系
            builder.HasOne(eas => eas.Application)
                .WithMany(asa => asa.EvaluateAfterSales)
                .HasForeignKey(eas => eas.ApplicationID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}