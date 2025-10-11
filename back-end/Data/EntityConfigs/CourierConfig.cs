using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 配送员实体配置
    /// </summary>
    public class CourierConfig : IEntityTypeConfiguration<Courier>
    {
        /// <summary>
        /// 配置配送员实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Courier> builder)
        {
            builder.ToTable("COURIERS");

            // 主键配置
            builder.HasKey(c => c.UserID);
            builder.Property(c => c.UserID).HasColumnName("USERID").ValueGeneratedNever();

            // 基础属性配置
            builder.Property(c => c.CourierRegistrationTime).HasColumnName("COURIERREGISTRATIONTIME").IsRequired();
            builder.Property(c => c.VehicleType).HasColumnName("VEHICLETYPE").IsRequired().HasMaxLength(20);
            builder.Property(c => c.ReputationPoints).HasColumnName("REPUTATIONPOINTS").HasDefaultValue(0);
            builder.Property(c => c.TotalDeliveries).HasColumnName("TOTALDELIVERIES").HasDefaultValue(0);
            builder.Property(c => c.AvgDeliveryTime).HasColumnName("AVGDELIVERYTIME").HasDefaultValue(0);
            builder.Property(c => c.AverageRating).HasColumnName("AVERAGERATING").HasColumnType("decimal(3,2)").HasDefaultValue(0.00m);
            builder.Property(c => c.MonthlySalary).HasColumnName("MONTHLYSALARY").HasDefaultValue(0);

            // 在线状态和位置配置
            builder.Property(c => c.IsOnline)
                .HasColumnName("ISONLINE")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(c => c.CourierLongitude)
                .HasColumnName("COURIERLONGITUDE")
                .HasColumnType("decimal(10,6)")
                .IsRequired(false);

            builder.Property(c => c.CourierLatitude)
                .HasColumnName("COURIERLATITUDE")
                .HasColumnType("decimal(10,6)")
                .IsRequired(false);

            builder.Property(c => c.LastOnlineTime).HasColumnName("LASTONLINETIME").IsRequired(false);
            builder.Property(c => c.CommissionThisMonth)
                .HasColumnName("COMMISSIONTHISMONTH")
                .HasColumnType("decimal(10,2)");

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Courier> builder)
        {
            // 配置与User的一对一关系
            builder.HasOne(c => c.User)
                .WithOne(u => u.Courier)
                .HasForeignKey<Courier>(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}