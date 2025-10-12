using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BackEnd.Models;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 菜单菜品种类关联实体配置
    /// </summary>
    public class Menu_DishCategoryConfig : IEntityTypeConfiguration<Menu_DishCategory>
    {
        /// <summary>
        /// 配置菜单菜品种类关联实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Menu_DishCategory> builder)
        {
            builder.ToTable("MENU_DISH_CATEGORY");

            // 复合主键配置
            builder.HasKey(mdc => new { mdc.MenuID, mdc.CategoryID });

            // 属性配置
            builder.Property(mdc => mdc.MenuID).HasColumnName("MENUID");
            builder.Property(mdc => mdc.CategoryID).HasColumnName("CATEGORYID");

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Menu_DishCategory> builder)
        {
            // 配置与Menu的多对一关系
            builder.HasOne(mdc => mdc.Menu)
                .WithMany(m => m.MenuDishCategories)
                .HasForeignKey(mdc => mdc.MenuID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与DishCategory的多对一关系
            builder.HasOne(mdc => mdc.DishCategory)
                .WithMany(dc => dc.MenuDishCategories)
                .HasForeignKey(mdc => mdc.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
