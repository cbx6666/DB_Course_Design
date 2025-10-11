using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 收藏项实体配置
    /// </summary>
    public class FavoriteItemConfig : IEntityTypeConfiguration<FavoriteItem>
    {
        /// <summary>
        /// 配置收藏项实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<FavoriteItem> builder)
        {
            builder.ToTable("FAVORITE_ITEMS");

            // 主键配置
            builder.HasKey(fi => fi.ItemID);
            builder.Property(fi => fi.ItemID).HasColumnName("ITEMID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(fi => fi.FavoritedAt).HasColumnName("FAVORITEDAT").IsRequired();
            builder.Property(fi => fi.FavoriteReason).HasColumnName("FAVORITEREASON").IsRequired().HasMaxLength(500);

            // 外键配置
            builder.Property(fi => fi.StoreID).HasColumnName("STOREID").IsRequired();
            builder.Property(fi => fi.FolderID).HasColumnName("FOLDERID").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<FavoriteItem> builder)
        {
            // 配置与Store的多对一关系
            builder.HasOne(fi => fi.Store)
                .WithMany(s => s.FavoriteItems)
                .HasForeignKey(fi => fi.StoreID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与FavoritesFolder的多对一关系
            builder.HasOne(fi => fi.Folder)
                .WithMany(f => f.FavoriteItems)
                .HasForeignKey(fi => fi.FolderID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}