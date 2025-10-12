using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 用户实体配置
    /// </summary>
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// 配置用户实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("USERS");

            // 主键和基础属性配置
            builder.HasKey(u => u.UserID);
            builder.Property(u => u.UserID).HasColumnName("USERID").ValueGeneratedOnAdd();

            builder.Property(u => u.Username).HasColumnName("USERNAME").IsRequired().HasMaxLength(15);
            builder.Property(u => u.Password).HasColumnName("PASSWORD").IsRequired().HasMaxLength(64);
            builder.Property(u => u.PhoneNumber).HasColumnName("PHONENUMBER").IsRequired();
            builder.Property(u => u.Email).HasColumnName("EMAIL").IsRequired().HasMaxLength(30);
            builder.Property(u => u.Gender).HasColumnName("GENDER").HasMaxLength(2);
            builder.Property(u => u.FullName).HasColumnName("FULLNAME").HasMaxLength(6);
            builder.Property(u => u.Avatar).HasColumnName("AVATAR").HasMaxLength(1000);
            builder.Property(u => u.Birthday).HasColumnName("BIRTHDAY");
            builder.Property(u => u.AccountCreationTime).HasColumnName("ACCOUNTCREATIONTIME").IsRequired();

            // 枚举类型配置

            builder.Property(u => u.Role)
                .HasColumnName("ROLE")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<User> builder)
        {
            // 配置与Customer的一对一关系
            builder.HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Courier的一对一关系
            builder.HasOne(u => u.Courier)
                .WithOne(c => c.User)
                .HasForeignKey<Courier>(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Administrator的一对一关系
            builder.HasOne(u => u.Administrator)
                .WithOne(a => a.User)
                .HasForeignKey<Administrator>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Seller的一对一关系
            builder.HasOne(u => u.Seller)
                .WithOne(s => s.User)
                .HasForeignKey<Seller>(s => s.UserID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
