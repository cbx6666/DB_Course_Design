using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 监督实体配置
    /// </summary>
    public class Supervise_Config : IEntityTypeConfiguration<Supervise_>
    {
        /// <summary>
        /// 配置监督实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Supervise_> builder)
        {
            builder.ToTable("SUPERVISE_");

            // 复合主键配置
            builder.HasKey(s => new { s.AdminID, s.PenaltyID });

            // 属性配置
            builder.Property(s => s.AdminID).HasColumnName("ADMINID");
            builder.Property(s => s.PenaltyID).HasColumnName("PENALTYID");

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Supervise_> builder)
        {
            // 配置与Administrator的多对一关系
            builder.HasOne(s => s.Admin)
                .WithMany(a => a.Supervise_s)
                .HasForeignKey(s => s.AdminID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与StoreViolationPenalty的多对一关系
            builder.HasOne(s => s.Penalty)
                .WithMany(svp => svp.Supervise_s)
                .HasForeignKey(s => s.PenaltyID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
