using BackEnd.Models;
using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 店铺违规处罚实体配置
    /// </summary>
    public class StoreViolationPenaltyConfig : IEntityTypeConfiguration<StoreViolationPenalty>
    {
        /// <summary>
        /// 配置店铺违规处罚实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<StoreViolationPenalty> builder)
        {
            builder.ToTable("STORE_VIOLATION_PENALTIES");

            // 主键配置
            builder.HasKey(svp => svp.PenaltyID);
            builder.Property(svp => svp.PenaltyID).HasColumnName("PENALTYID").ValueGeneratedOnAdd();

            // 状态配置
            builder.Property(svp => svp.ViolationPenaltyState)
                .HasColumnName("VIOLATIONPENALTYSTATE")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(ViolationPenaltyState.Pending);

            // 基础属性配置
            builder.Property(svp => svp.PenaltyReason).HasColumnName("PENALTYREASON").IsRequired().HasMaxLength(255);
            builder.Property(svp => svp.PenaltyNote).HasColumnName("PENALTYNOTE").IsRequired(false).HasMaxLength(255);
            builder.Property(svp => svp.PenaltyTime).HasColumnName("PENALTYTIME").IsRequired();
            builder.Property(svp => svp.SellerPenalty).HasColumnName("SELLERPENALTY").HasMaxLength(50);
            builder.Property(svp => svp.StorePenalty).HasColumnName("STOREPENALTY").HasMaxLength(50);

            // 外键配置
            builder.Property(svp => svp.StoreID).HasColumnName("STOREID").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<StoreViolationPenalty> builder)
        {
            // 配置与Store的多对一关系
            builder.HasOne(svp => svp.Store)
                .WithMany(s => s.StoreViolationPenalties)
                .HasForeignKey(svp => svp.StoreID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
