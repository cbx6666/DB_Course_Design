using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BackEnd.Models;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 菜单菜品关联实体配置
    /// </summary>
    public class Menu_DishConfig : IEntityTypeConfiguration<Menu_Dish>
    {
        /// <summary>
        /// 配置菜单菜品关联实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Menu_Dish> builder)
        {
            builder.ToTable("MENU_DISH");

            // 复合主键配置
            builder.HasKey(md => new { md.MenuID, md.DishID });

            // 属性配置
            builder.Property(md => md.MenuID).HasColumnName("MENUID");
            builder.Property(md => md.DishID).HasColumnName("DISHID");

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Menu_Dish> builder)
        {
            // 配置与Menu的多对一关系
            builder.HasOne(md => md.Menu)
                .WithMany(m => m.MenuDishes)
                .HasForeignKey(md => md.MenuID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Dish的多对一关系
            builder.HasOne(md => md.Dish)
                .WithMany(d => d.MenuDishes)
                .HasForeignKey(md => md.DishID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}