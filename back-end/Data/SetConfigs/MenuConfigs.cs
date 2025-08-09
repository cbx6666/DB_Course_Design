using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BackEnd.Models;

namespace BackEnd.Data.SetConfigs
{
    public class MenuConfig : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("MENU");

            builder.HasKey(m => m.MenuID);

            builder.Property(m => m.MenuID).HasColumnName("MENUID").ValueGeneratedOnAdd();

            builder.Property(m => m.Version).HasColumnName("VERSION").IsRequired().HasMaxLength(50);

            builder.Property(m => m.ActivePeriod).HasColumnName("ACTIVEPERIOD").IsRequired();

            builder.Property(m => m.StoreID).HasColumnName("STOREID").IsRequired();

            // ---------------------------------------------------------------
            // ��ϵ����
            // ---------------------------------------------------------------

            // ��ϵһ: Menu -> Store (���һ)
            builder.HasOne(m => m.Store)
                   .WithMany(s => s.Menus)
                   .HasForeignKey(m => m.StoreID)
                   .OnDelete(DeleteBehavior.Cascade); // ���̵걻ɾ��ʱ�������в˵�ҲӦ������ɾ��

            // ��ϵ��: Menu -> Menu_Dish (һ�Զ࣬����ʵ�ֶ�Զ�)
            builder.HasMany(m => m.MenuDishes)
                   .WithOne(md => md.Menu) 
                   .HasForeignKey(md => md.MenuID)
                   .OnDelete(DeleteBehavior.Cascade); // ���˵���ɾ��ʱ�������в�Ʒ������¼��Ӧ������ɾ��
        }
    }
}