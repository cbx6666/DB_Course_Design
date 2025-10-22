using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BackEnd.Models;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 菜单实体配置
    /// </summary>
    public class MenuConfig : IEntityTypeConfiguration<Menu>
    {
        /// <summary>
        /// 配置菜单实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("MENUS");

            // 主键配置
            builder.HasKey(m => m.MenuID);
            builder.Property(m => m.MenuID).HasColumnName("MENUID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(m => m.Name).HasColumnName("NAME").IsRequired().HasMaxLength(100);
            builder.Property(m => m.Description).HasColumnName("DESCRIPTION").IsRequired().HasMaxLength(500);
            builder.Property(m => m.IsActive).HasColumnName("ISACTIVE").IsRequired().HasConversion<int>();
            builder.Property(m => m.CreatedAt).HasColumnName("CREATEDAT").IsRequired();

            // 外键配置
            builder.Property(m => m.StoreID).HasColumnName("STOREID").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Menu> builder)
        {
            // 配置与Store的多对一关系
            builder.HasOne(m => m.Store)
                .WithMany(s => s.Menus)
                .HasForeignKey(m => m.StoreID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}