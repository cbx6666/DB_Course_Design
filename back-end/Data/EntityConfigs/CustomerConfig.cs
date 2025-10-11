using BackEnd.Models;
using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 消费者实体配置
    /// </summary>
    public class CustomerConfig : IEntityTypeConfiguration<Customer>
    {
        /// <summary>
        /// 配置消费者实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("CUSTOMERS");

            // 主键配置
            builder.HasKey(c => c.UserID);
            builder.Property(c => c.UserID).HasColumnName("USERID").ValueGeneratedNever();

            // 基础属性配置
            builder.Property(c => c.DefaultAddress).HasColumnName("DEFAULTADDRESS").HasMaxLength(100);
            builder.Property(c => c.ReputationPoints).HasColumnName("REPUTATIONPOINTS").HasDefaultValue(0);

            // 会员状态配置
            builder.Property(c => c.IsMember)
                .HasColumnName("ISMEMBER")
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(MembershipStatus.NotMember);

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Customer> builder)
        {
            // 配置与User的一对一关系
            builder.HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}