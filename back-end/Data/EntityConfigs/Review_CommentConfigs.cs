using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BackEnd.Models;

namespace BackEnd.Data.EntityConfigs
{
    /// <summary>
    /// 评论审核实体配置
    /// </summary>
    public class Review_CommentConfig : IEntityTypeConfiguration<Review_Comment>
    {
        /// <summary>
        /// 配置评论审核实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Review_Comment> builder)
        {
            builder.ToTable("REVIEW_COMMENT");

            // 复合主键配置
            builder.HasKey(rc => new { rc.AdminID, rc.CommentID });

            // 属性配置
            builder.Property(rc => rc.AdminID).HasColumnName("ADMINID");
            builder.Property(rc => rc.CommentID).HasColumnName("COMMENTID");
            builder.Property(rc => rc.ReviewTime).HasColumnName("REVIEWTIME").IsRequired();

            // 关系配置
            ConfigureRelationships(builder);
        }

        /// <summary>
        /// 配置实体关系
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        private static void ConfigureRelationships(EntityTypeBuilder<Review_Comment> builder)
        {
            // 配置与Administrator的多对一关系
            builder.HasOne(rc => rc.Admin)
                .WithMany(a => a.ReviewComments)
                .HasForeignKey(rc => rc.AdminID)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置与Comment的多对一关系
            builder.HasOne(rc => rc.Comment)
                .WithMany(c => c.ReviewComments)
                .HasForeignKey(rc => rc.CommentID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}