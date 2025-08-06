using BackEnd.Models;
using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.EntityConfigs
{
    public class DishConfig : IEntityTypeConfiguration<Dish>
    {
        public void Configure(EntityTypeBuilder<Dish> builder)
        {
            builder.ToTable("DISHES");

            builder.HasKey(d => d.DishID);

            builder.Property(d => d.DishID).HasColumnName("DISHID").ValueGeneratedOnAdd();

            builder.Property(d => d.DishName).HasColumnName("DISHNAME").IsRequired().HasMaxLength(50);
            
            builder.Property(d => d.Price).HasColumnName("PRICE").IsRequired().HasColumnType("decimal(10,2)");
            
            builder.Property(d => d.Description).HasColumnName("DESCRIPTION").IsRequired().HasMaxLength(500);

            builder.Property(d => d.IsSoldOut)
                   .HasColumnName("ISSOLDOUT")
                   .IsRequired()
                   .HasConversion<string>() // ��ö�ٴ洢Ϊ�ַ������� "IsSoldOut", "Available"�������߿ɶ���
                   .HasMaxLength(20)
                   .HasDefaultValue(DishIsSoldOut.IsSoldOut); // ʹ��ǿ����ö������Ĭ��ֵ

            // ---------------------------------------------------------------
            // ��ϵ����
            // ---------------------------------------------------------------

            // ������ Menu �Ķ�Զ��ϵ (ͨ�� Menu_Dish �м��)
            // һ����Ʒ���Գ����ڶ�� Menu_Dish ��¼��
            builder.HasMany(d => d.MenuDishes)
                   .WithOne(md => md.Dish) // ÿ�� Menu_Dish ��¼��Ӧһ����Ʒ
                   .HasForeignKey(md => md.DishID) // ����� Menu_Dish ����
                   .OnDelete(DeleteBehavior.Cascade); // ����Ʒ��ɾ��ʱ���������в˵��еļ�¼ҲӦ��ɾ��
        }
    }
}