using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.SetConfigs
{
    public class ShoppingCartItemConfig : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            builder.ToTable("SHOPPINGCARTITEM");

            // --- �����ͻ����������� ---
            builder.HasKey(sci => sci.ItemID);
            builder.Property(sci => sci.ItemID).HasColumnName("ITEMID").ValueGeneratedOnAdd();

            builder.Property(sci => sci.Quantity).HasColumnName("QUANTITY").IsRequired();
            builder.Property(sci => sci.TotalPrice).HasColumnName("TOTALPRICE").HasColumnType("decimal(10,2)");

            // --- ����������� ---
            builder.Property(sci => sci.DishID).HasColumnName("DISHID").IsRequired();
            builder.Property(sci => sci.CartID).HasColumnName("CARTID").IsRequired();

            // ---------------------------------------------------------------
            // ��ϵ����
            // ---------------------------------------------------------------

            // ��ϵһ: ShoppingCartItem �� Dish (���һ)
            // ������ﳵ�����ָ��ͬһ����Ʒ
            builder.HasOne(sci => sci.Dish) // һ�����ﳵ��ֻ��һ����Ʒ
                   .WithMany(d => d.ShoppingCartItems)
                   .HasForeignKey(sci => sci.DishID)
                   .OnDelete(DeleteBehavior.Restrict); // ��ֹ��Ʒ��ɾ�������������ĳ�˵Ĺ��ﳵ�

            // ��ϵ��: ShoppingCartItem �� ShoppingCart (���һ)
            // ������ﳵ������ͬһ�����ﳵ
            builder.HasOne(sci => sci.Cart) // һ�����ﳵ��ֻ����һ�����ﳵ
                   .WithMany(sc => sc.ShoppingCartItems) // һ�����ﳵӵ�ж�����ﳵ�� (ShoppingCartItems ����)
                   .HasForeignKey(sci => sci.CartID) // ����� CartID
                   .OnDelete(DeleteBehavior.Cascade); // �ؼ��������ﳵ��ɾ��ʱ����������ҲӦ������ɾ����
        }
    }
}
