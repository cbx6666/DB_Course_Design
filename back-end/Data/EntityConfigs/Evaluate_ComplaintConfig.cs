using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 配送投诉评估实体配置
    /// </summary>
    public class Evaluate_ComplaintConfig : IEntityTypeConfiguration<Evaluate_Complaint>
    {
        /// <summary>
        /// 配置配送投诉评估实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Evaluate_Complaint> builder)
        {
            builder.ToTable("EVALUATE_COMPLAINT");

            // 复合主键配置
            builder.HasKey(ec => new { ec.AdminID, ec.ComplaintID });

            // 属性配置
            builder.Property(ec => ec.AdminID).HasColumnName("ADMINID");
            builder.Property(ec => ec.ComplaintID).HasColumnName("COMPLAINTID");

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Evaluate_Complaint> builder)
        {
            // 配置与Administrator的多对一关系
            builder.HasOne(ec => ec.Admin)
                .WithMany(a => a.EvaluateComplaints)
                .HasForeignKey(ec => ec.AdminID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与DeliveryComplaint的多对一关系
            builder.HasOne(ec => ec.Complaint)
                .WithMany(dc => dc.EvaluateComplaints)
                .HasForeignKey(ec => ec.ComplaintID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}