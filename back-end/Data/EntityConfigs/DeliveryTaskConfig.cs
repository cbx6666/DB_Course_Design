using BackEnd.Models;
using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 配送任务实体配置
    /// </summary>
    public class DeliveryTaskConfig : IEntityTypeConfiguration<DeliveryTask>
    {
        /// <summary>
        /// 配置配送任务实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<DeliveryTask> builder)
        {
            builder.ToTable("DELIVERY_TASKS");

            // 主键配置
            builder.HasKey(dt => dt.TaskID);
            builder.Property(dt => dt.TaskID).HasColumnName("TASKID").ValueGeneratedOnAdd();

            // 时间相关属性配置
            builder.Property(dt => dt.EstimatedArrivalTime).HasColumnName("ESTIMATEDARRIVALTIME").IsRequired();
            builder.Property(dt => dt.EstimatedDeliveryTime).HasColumnName("ESTIMATEDDELIVERYTIME").IsRequired();
            builder.Property(dt => dt.PublishTime).HasColumnName("PUBLISHTIME").IsRequired();
            builder.Property(dt => dt.AcceptTime).HasColumnName("ACCEPTTIME").IsRequired();
            builder.Property(dt => dt.CompletionTime).HasColumnName("COMPLETIONTIME").IsRequired(false);

            // 状态和费用配置
            builder.Property(dt => dt.Status)
                .HasColumnName("STATUS")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(DeliveryStatus.To_Be_Taken);

            builder.Property(dt => dt.DeliveryFee)
                .HasColumnName("DELIVERYFEE")
                .HasColumnType("decimal(5,2)")
                .IsRequired()
                .HasDefaultValue(0.00m);

            // 外键配置
            builder.Property(dt => dt.CustomerID).HasColumnName("CUSTOMERID").IsRequired();
            builder.Property(dt => dt.StoreID).HasColumnName("STOREID").IsRequired();
            builder.Property(dt => dt.CourierID).HasColumnName("COURIERID").IsRequired(false);
            builder.Property(dt => dt.OrderID).HasColumnName("ORDERID").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<DeliveryTask> builder)
        {
            // 配置与Customer的多对一关系
            builder.HasOne(dt => dt.Customer)
                .WithMany(c => c.DeliveryTasks)
                .HasForeignKey(dt => dt.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置与Store的多对一关系
            builder.HasOne(dt => dt.Store)
                .WithMany(s => s.DeliveryTasks)
                .HasForeignKey(dt => dt.StoreID)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置与Courier的多对一关系
            builder.HasOne(dt => dt.Courier)
                .WithMany(c => c.DeliveryTasks)
                .HasForeignKey(dt => dt.CourierID)
                .OnDelete(DeleteBehavior.SetNull);

            // 配置与FoodOrder的一对一关系
            builder.HasOne(dt => dt.Order)
                .WithOne(fd => fd.DeliveryTask)
                .HasForeignKey<DeliveryTask>(dt => dt.OrderID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}