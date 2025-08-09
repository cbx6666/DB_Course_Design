using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.SetConfigs
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("USER");

            // --- �����ͻ����������� ---
            builder.HasKey(u => u.UserID);
            builder.Property(u => u.UserID).HasColumnName("USERID").ValueGeneratedOnAdd();

            builder.Property(u => u.Username).HasColumnName("USERNAME").IsRequired().HasMaxLength(15);

            // ���飺����洢���ǹ�ϣֵ������ͨ���ϳ���128λ��һ�������ѡ��
            builder.Property(u => u.Password).HasColumnName("PASSWORD").IsRequired().HasMaxLength(128);

            // ������PhoneNumber �� long ���ͣ���Ӧ�� MaxLength ���ơ�
            builder.Property(u => u.PhoneNumber).HasColumnName("PHONENUMBER").IsRequired();

            builder.Property(u => u.Email).HasColumnName("EMAIL").IsRequired().HasMaxLength(30);

            // ��������ģ�� [MaxLength(2)] ����һ��
            builder.Property(u => u.Gender).HasColumnName("GENDER").HasMaxLength(2);

            // ��������ģ�� [MaxLength(6)] ����һ��
            builder.Property(u => u.FullName).HasColumnName("FULLNAME").HasMaxLength(6);

            builder.Property(u => u.Avatar).HasColumnName("AVATAR").HasMaxLength(255);
            builder.Property(u => u.Birthday).HasColumnName("BIRTHDAY");
            builder.Property(u => u.AccountCreationTime).HasColumnName("ACCOUNTCREATIONTIME").IsRequired();

            // --- ö���������� ---
            // ��ö�ٴ洢Ϊ�ַ������������ݿ��Ķ��͵���
            builder.Property(u => u.IsProfilePublic).HasColumnName("ISPROFILEPUBLIC").IsRequired().HasConversion<string>().HasMaxLength(20);
            builder.Property(u => u.Role).HasColumnName("ROLE").IsRequired().HasConversion<string>().HasMaxLength(20);

            // ---------------------------------------------------------------
            // ��ϵ���ã��û���Ϊ�����������ɫ�ӱ���һ��һ��ϵ
            // ---------------------------------------------------------------

            // ��ϵһ: User -> Customer (һ��һ)
            builder.HasOne(u => u.Customer)
                   .WithOne(c => c.User) // �� Customer ģ���У����򵼺�����Ϊ User
                   .HasForeignKey<Customer>(c => c.UserID) // ����� Customer ����
                   .OnDelete(DeleteBehavior.Cascade); // �� User ��ɾ��ʱ��������� Customer ���ҲӦ������ɾ����

            // ��ϵ��: User -> Courier (һ��һ)
            builder.HasOne(u => u.Courier)
                   .WithOne(c => c.User) // �� Courier ģ���У����򵼺�����Ϊ User
                   .HasForeignKey<Courier>(c => c.UserID) // ����� Courier ����
                   .OnDelete(DeleteBehavior.Cascade); // �� User ��ɾ��ʱ��������� Courier ���ҲӦ������ɾ����

            // ��ϵ��: User -> Administrator (һ��һ)
            builder.HasOne(u => u.Administrator)
                   .WithOne(a => a.User) // �� Administrator ģ���У����򵼺�����Ϊ User
                   .HasForeignKey<Administrator>(a => a.UserID) // ����� Administrator ����
                   .OnDelete(DeleteBehavior.Cascade); // �� User ��ɾ��ʱ��������� Administrator ���ҲӦ������ɾ����

            // ��ϵ��: User -> Seller (һ��һ)
            builder.HasOne(u => u.Seller)
                   .WithOne(s => s.User) // �� Seller ģ���У����򵼺�����Ϊ User
                   .HasForeignKey<Seller>(s => s.UserID) // ����� Seller ����
                   .OnDelete(DeleteBehavior.Cascade); // �� User ��ɾ��ʱ��������� Seller ���ҲӦ������ɾ����
        }
    }
}
