using BackEnd.Models;
using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 配送投诉实体配置
    /// </summary>
    public class DeliveryComplaintConfig : IEntityTypeConfiguration<DeliveryComplaint>
    {
        /// <summary>
        /// 配置配送投诉实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<DeliveryComplaint> builder)
        {
            builder.ToTable("DELIVERY_COMPLAINTS");

            // 主键配置
            builder.HasKey(dc => dc.ComplaintID);
            builder.Property(dc => dc.ComplaintID).HasColumnName("COMPLAINTID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(dc => dc.ComplaintReason).HasColumnName("COMPLAINTREASON").IsRequired().HasMaxLength(255);
            builder.Property(dc => dc.ComplaintTime).HasColumnName("COMPLAINTTIME").IsRequired();

            // 状态配置
            builder.Property(dc => dc.ComplaintState)
                .HasColumnName("COMPLAINTSTATE")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(ComplaintState.Pending);

            // 处理相关属性配置
            builder.Property(dc => dc.ProcessingResult).HasColumnName("PROCESSINGRESULT").IsRequired(false).HasMaxLength(255);
            builder.Property(dc => dc.ProcessingReason).HasColumnName("PROCESSINGREASON").IsRequired(false).HasMaxLength(255);
            builder.Property(dc => dc.ProcessingRemark).HasColumnName("PROCESSINGREMARK").IsRequired(false).HasMaxLength(255);
            builder.Property(dc => dc.FineAmount)
                .HasColumnName("FINEAMOUNT")
                .IsRequired(false)
                .HasColumnType("decimal(18, 2)");

            // 外键配置
            builder.Property(dc => dc.CourierID).HasColumnName("COURIERID").IsRequired();
            builder.Property(dc => dc.CustomerID).HasColumnName("CUSTOMERID").IsRequired();
            builder.Property(dc => dc.DeliveryTaskID).HasColumnName("DELIVERYTASKID").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<DeliveryComplaint> builder)
        {
            // 配置与DeliveryTask的多对一关系
            builder.HasOne(dc => dc.DeliveryTask)
                .WithMany(dt => dt.DeliveryComplaints)
                .HasForeignKey(dc => dc.DeliveryTaskID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}