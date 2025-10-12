using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BackEnd.Models;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 菜品种类实体配置
    /// </summary>
    public class DishCategoryConfig : IEntityTypeConfiguration<DishCategory>
    {
        /// <summary>
        /// 配置菜品种类实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<DishCategory> builder)
        {
            builder.ToTable("DISH_CATEGORIES");

            // 主键配置
            builder.HasKey(dc => dc.CategoryID);
            builder.Property(dc => dc.CategoryID).HasColumnName("CATEGORYID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(dc => dc.CategoryName).HasColumnName("CATEGORYNAME").IsRequired().HasMaxLength(50);
        }
    }
}
