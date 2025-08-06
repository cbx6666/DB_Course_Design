using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BackEnd.Models;

namespace BackEnd.Data.SetConfigs
{
    public class Review_CommentConfig : IEntityTypeConfiguration<Review_Comment>
    {
        public void Configure(EntityTypeBuilder<Review_Comment> builder)
        {
            builder.ToTable("REVIEW_COMMENT");

            builder.HasKey(rc => new { rc.AdminID, rc.CommentID });

            builder.Property(rc => rc.AdminID).HasColumnName("ADMINID");

            builder.Property(rc => rc.CommentID).HasColumnName("COMMENTID");

            builder.Property(rc => rc.ReviewTime).HasColumnName("REVIEWTIME").IsRequired();

            // ---------------------------------------------------------------
            // ��ϵ����
            // ---------------------------------------------------------------

            // ��ϵһ: Review_Comment -> Administrator (���һ)
            builder.HasOne(rc => rc.Admin)
                   .WithMany(a => a.ReviewComments) 
                   .HasForeignKey(rc => rc.AdminID)
                   .OnDelete(DeleteBehavior.Restrict); // ��ֹͨ��ɾ����˼�¼��ɾ������Ա

            // ��ϵ��: Review_Comment -> Comment (���һ)
            builder.HasOne(rc => rc.Comment)
                   .WithMany(c => c.ReviewComments) 
                   .HasForeignKey(rc => rc.CommentID)
                   .OnDelete(DeleteBehavior.Restrict); // ��ֹͨ��ɾ����˼�¼��ɾ������
        }
    }
}