using BackEnd.Models;
using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 评论实体配置
    /// </summary>
    public class CommentConfig : IEntityTypeConfiguration<Comment>
    {
        /// <summary>
        /// 配置评论实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("COMMENTS");

            // 主键配置
            builder.HasKey(c => c.CommentID);
            builder.Property(c => c.CommentID).HasColumnName("COMMENTID").ValueGeneratedOnAdd();

            // 基础属性配置
            builder.Property(c => c.Content).HasColumnName("CONTENT").IsRequired().HasMaxLength(500);
            builder.Property(c => c.PostedAt).HasColumnName("POSTEDAT").IsRequired();
            builder.Property(c => c.Likes).HasColumnName("LIKES").HasDefaultValue(0);
            builder.Property(c => c.Replies).HasColumnName("REPLIES").HasDefaultValue(0);
            builder.Property(c => c.Rating).HasColumnName("RATING").IsRequired(false);
            builder.Property(c => c.CommentImage).HasColumnName("COMMENTIMAGE").HasMaxLength(1000).IsRequired(false);

            // 类型和状态配置
            builder.Property(c => c.CommentType).HasColumnName("COMMENTTYPE").IsRequired().HasConversion<int>();
            builder.Property(c => c.CommentState)
                .HasColumnName("COMMENTSTATE")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(CommentState.Pending);

            // 外键配置
            builder.Property(c => c.ReplyToCommentID).HasColumnName("REPLYTOCOMMENTID").IsRequired(false);
            builder.Property(c => c.StoreID).HasColumnName("STOREID").IsRequired(false);
            builder.Property(c => c.FoodOrderID).HasColumnName("FOODORDERID").IsRequired(false);
            builder.Property(c => c.CommenterID).HasColumnName("COMMENTERID").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Comment> builder)
        {
            // 配置与Store的多对一关系
            builder.HasOne(c => c.Store)
                .WithMany(s => s.Comments)
                .HasForeignKey(c => c.StoreID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Customer的多对一关系
            builder.HasOne(c => c.Commenter)
                .WithMany(cu => cu.Comments)
                .HasForeignKey(c => c.CommenterID)
                .OnDelete(DeleteBehavior.SetNull);

            // 配置自引用关系（回复评论）
            builder.HasOne(c => c.ReplyToComment)
                .WithMany(rc => rc.CommentReplies)
                .HasForeignKey(c => c.ReplyToCommentID)
                .OnDelete(DeleteBehavior.SetNull);

            // 配置与FoodOrder的多对一关系
            builder.HasOne(c => c.FoodOrder)
                .WithMany(fo => fo.Comments)
                .HasForeignKey(c => c.FoodOrderID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}