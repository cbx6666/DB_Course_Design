using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 管理员实体配置
    /// </summary>
    public class AdministratorConfig : IEntityTypeConfiguration<Administrator>
    {
        /// <summary>
        /// 配置管理员实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Administrator> builder)
        {
            builder.ToTable("ADMINISTRATORS");

            // 主键配置
            builder.HasKey(a => a.UserID);
            builder.Property(a => a.UserID).HasColumnName("USERID").ValueGeneratedNever();

            // 基础属性配置
            builder.Property(a => a.AdminRegistrationTime).HasColumnName("ADMINREGISTRATIONTIME").IsRequired();
            builder.Property(a => a.AdminRole).HasColumnName("ADMINROLE").IsRequired().HasMaxLength(20);
            builder.Property(a => a.ManagedEntities).HasColumnName("MANAGEDENTITIES").IsRequired().HasMaxLength(50);
            builder.Property(a => a.IssueHandlingScore)
                .HasColumnName("ISSUEHANDLINGSCORE")
                .HasColumnType("decimal(3,2)")
                .HasDefaultValue(0.00m);

            // 关系配置
            ConfigureRelationships(builder);

            // 忽略计算属性
            builder.Ignore(a => a.Comments);
            builder.Ignore(a => a.Penalties);
            builder.Ignore(a => a.Applications);
            builder.Ignore(a => a.DeliveryComplaints);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Administrator> builder)
        {
            // 配置与User的一对一关系
            builder.HasOne(a => a.User)
                .WithOne(u => u.Administrator)
                .HasForeignKey<Administrator>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}