using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.SetConfigs
{
    public class StoreViolationPenaltyConfig : IEntityTypeConfiguration<StoreViolationPenalty>
    {
        public void Configure(EntityTypeBuilder<StoreViolationPenalty> builder)
        {
            builder.ToTable("STOREVIOLATIONPENALTIE");

            // --- �����ͻ����������� ---
            builder.HasKey(svp => svp.PenaltyID);
            builder.Property(svp => svp.PenaltyID).HasColumnName("PENALTYID").ValueGeneratedOnAdd();

            builder.Property(svp => svp.PenaltyReason).HasColumnName("PENALTYREASON").IsRequired().HasMaxLength(255);
            builder.Property(svp => svp.PenaltyTime).HasColumnName("PENALTYTIME").IsRequired();
            builder.Property(svp => svp.SellerPenalty).HasColumnName("SELLERPENALTY").HasMaxLength(50);
            builder.Property(svp => svp.StorePenalty).HasColumnName("STOREPENALTY").HasMaxLength(50);

            // --- ����������� ---
            builder.Property(svp => svp.StoreID).HasColumnName("STOREID").IsRequired();

            // ---------------------------------------------------------------
            // ��ϵ����
            // ---------------------------------------------------------------

            // ��ϵһ: StoreViolationPenalty -> Store (���һ)
            // ���������¼���Թ�����ͬһ�����̡�
            builder.HasOne(svp => svp.Store)
                   .WithMany(s => s.StoreViolationPenalties) 
                   .HasForeignKey(svp => svp.StoreID)
                   .OnDelete(DeleteBehavior.Restrict); // ������ɾ��һ������Υ���¼�ĵ��̣��Ա���������ʷ������ StoreConfig �е����ñ���һ�¡�

            // ��ϵ��: StoreViolationPenalty -> Supervise_ (һ�Զ�)
            // ����ʵ���� Administrator ��Զ��ϵ�����Ӳ��֡�һ��������¼�����ɶ������Ա��ܡ�
            builder.HasMany(svp => svp.Supervise_s)
                   .WithOne(s => s.Penalty) // ���� Supervise_ ģ������ public StoreViolationPenalty Penalty { get; set; }
                   .HasForeignKey(s => s.PenaltyID) 
                   .OnDelete(DeleteBehavior.Cascade); // ��һ��������¼��ɾ��ʱ�������������Ա�Ĺ�����¼���� Supervise_ ���У�ҲӦ������ɾ����
        }
    }
}
