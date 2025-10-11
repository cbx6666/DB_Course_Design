using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 收藏夹实体配置
    /// </summary>
    public class FavoritesFolderConfig : IEntityTypeConfiguration<FavoritesFolder>
    {
        /// <summary>
        /// 配置收藏夹实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<FavoritesFolder> builder)
        {
            builder.ToTable("FAVORITES_FOLDERS");

            // 主键配置
            builder.HasKey(ff => ff.FolderID);
            builder.Property(ff => ff.FolderID).HasColumnName("FOLDERID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(ff => ff.FolderName).HasColumnName("FOLDERNAME").IsRequired().HasMaxLength(50);

            // 外键配置
            builder.Property(ff => ff.CustomerID).HasColumnName("CUSTOMERID").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<FavoritesFolder> builder)
        {
            // 配置与Customer的多对一关系
            builder.HasOne(f => f.Customer)
                .WithMany(c => c.FavoritesFolders)
                .HasForeignKey(f => f.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}