using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.SetConfigs
{
    public class SellerConfig : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.ToTable("SELLER");

            builder.HasKey(s => s.UserID);  

            builder.Property(s => s.UserID).HasColumnName("USERID").ValueGeneratedNever();

            builder.Property(s => s.SellerRegistrationTime).HasColumnName("SELLERREGISTRATIONTIME").IsRequired();

            builder.Property(s => s.ReputationPoints).HasColumnName("REPUTATIONPOINTS").HasDefaultValue(0);

            builder.Property(s => s.BanStatus).HasColumnName("BANSTATUS").IsRequired().HasMaxLength(10);

            // ---------------------------------------------------------------
            // ��ϵ����
            // ---------------------------------------------------------------

            // ��ϵһ: Seller �� User (һ��һ�̳й�ϵ)
            builder.HasOne(s => s.User)
                   .WithOne(u => u.Seller)
                   .HasForeignKey<Seller>(s => s.UserID)
                   .OnDelete(DeleteBehavior.Cascade); // �� User ��ɾ��ʱ���� Seller ���ҲӦ����ɾ��

            // ��ϵ��: Seller �� Store (һ��һ��ѡ��ϵ)
            builder.HasOne(s => s.Store)
                   .WithOne(st => st.Seller)
                   .HasForeignKey<Store>(st => st.SellerID)
                   .OnDelete(DeleteBehavior.Cascade); // �� Seller ��ɾ��ʱ����ӵ�е� Store ҲӦ����ɾ��
        }
    }
}