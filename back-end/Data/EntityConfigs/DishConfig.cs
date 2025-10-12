using BackEnd.Models;
using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 菜品实体配置
    /// </summary>
    public class DishConfig : IEntityTypeConfiguration<Dish>
    {
        /// <summary>
        /// 配置菜品实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Dish> builder)
        {
            builder.ToTable("DISHES");

            // 主键配置
            builder.HasKey(d => d.DishID);
            builder.Property(d => d.DishID).HasColumnName("DISHID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(d => d.DishName).HasColumnName("DISHNAME").IsRequired().HasMaxLength(50);
            builder.Property(d => d.Price).HasColumnName("PRICE").IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(d => d.Description).HasColumnName("DESCRIPTION").IsRequired().HasMaxLength(500);
            builder.Property(d => d.DishImage).HasColumnName("DISHIMAGE").HasMaxLength(500).IsRequired(false);
            builder.Property(d => d.CategoryID).HasColumnName("CATEGORYID").IsRequired();

            // 售罄状态配置
            builder.Property(d => d.IsSoldOut)
                .HasColumnName("ISSOLDOUT")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(DishIsSoldOut.IsSoldOut);

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Dish> builder)
        {
            // 配置与DishCategory的多对一关系
            builder.HasOne(d => d.DishCategory)
                .WithMany(dc => dc.Dishes)
                .HasForeignKey(d => d.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}