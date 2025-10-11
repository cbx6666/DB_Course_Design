using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 商家实体配置
    /// </summary>
    public class SellerConfig : IEntityTypeConfiguration<Seller>
    {
        /// <summary>
        /// 配置商家实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.ToTable("SELLERS");

            // 主键配置
            builder.HasKey(s => s.UserID);
            builder.Property(s => s.UserID).HasColumnName("USERID").ValueGeneratedNever();

            // 基础属性配置
            builder.Property(s => s.SellerRegistrationTime).HasColumnName("SELLERREGISTRATIONTIME").IsRequired();
            builder.Property(s => s.ReputationPoints).HasColumnName("REPUTATIONPOINTS").HasDefaultValue(0);

            // 状态配置
            builder.Property(s => s.BanStatus)
                .HasColumnName("BANSTATUS")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Seller> builder)
        {
            // 配置与User的一对一关系
            builder.HasOne(s => s.User)
                .WithOne(u => u.Seller)
                .HasForeignKey<Seller>(s => s.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}