using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.SetConfigs
{
    public class SuperviseConfig : IEntityTypeConfiguration<Supervise>
    {
        public void Configure(EntityTypeBuilder<Supervise> builder)
        {
            builder.ToTable("SUPERVISE");

            // --- �������ã��������� ---
            // Supervise ��ʹ�� AdminID �� PenaltyID ��ͬ����һ��Ψһ�ĸ���������
            // ��ȷ����ͬһ������Ա�����ظ�������ͬһ��������¼��
            builder.HasKey(s => new { s.AdminID, s.PenaltyID });

            // --- �������� ---
            builder.Property(s => s.AdminID).HasColumnName("ADMINID");
            builder.Property(s => s.PenaltyID).HasColumnName("PENALTYID");

            // ---------------------------------------------------------------
            // ��ϵ����
            // ---------------------------------------------------------------

            // ��ϵһ: Supervise -> Administrator (���һ)
            // ����ල��¼����ָ��ͬһ������Ա��
            builder.HasOne(s => s.Admin)
                   .WithMany(a => a.Supervises) // �� Administrator ģ���У����򵼺�������Ϊ Supervises
                   .HasForeignKey(s => s.AdminID)
                   .OnDelete(DeleteBehavior.Cascade); // �ؼ���������Ա��ɾ��ʱ�������мල��¼ҲӦ������ɾ����

            // ��ϵ��: Supervise -> StoreViolationPenalty (���һ)
            // ����ල��¼����ָ��ͬһ��������¼��
            builder.HasOne(s => s.Penalty)
                   .WithMany(svp => svp.Supervises) // �� StoreViolationPenalty ģ���У����򵼺�������Ϊ Supervises
                   .HasForeignKey(s => s.PenaltyID)
                   .OnDelete(DeleteBehavior.Cascade); // �ؼ�����������¼��ɾ��ʱ�������мල��¼ҲӦ������ɾ����
        }
    }
}
