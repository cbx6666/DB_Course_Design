using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.SetConfigs
{
    public class ShoppingCartConfig : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.ToTable("SHOPPINGCART");

            builder.HasKey(sc => sc.CartID);

            builder.Property(sc => sc.CartID).HasColumnName("CARTID").ValueGeneratedOnAdd();

            builder.Property(sc => sc.LastUpdatedTime).HasColumnName("LASTUPDATEDTIME").IsRequired();

            builder.Property(sc => sc.TotalPrice).HasColumnName("TOTALPRICE").HasColumnType("decimal(10,2)").HasDefaultValue(0.00m);

            builder.Property(sc => sc.OrderID).HasColumnName("ORDERID");

            // ---------------------------------------------------------------
            // ��ϵ����
            // ---------------------------------------------------------------

            // ��ϵһ: ShoppingCart �� FoodOrder (һ��һ����ѡ��ϵ)
            builder.HasOne(sc => sc.Order)
                   .WithOne(o => o.Cart)
                   .HasForeignKey<ShoppingCart>(sc => sc.OrderID)
                   .OnDelete(DeleteBehavior.SetNull); // ���������ɾ�����������ﳵ�� OrderID ��Ϊ NULL����ɾ�����ﳵ����

            // ��ϵ��: ShoppingCart �� ShoppingCartItem (һ�Զ�)
            // һ�����ﳵ����������ﳵ�
            builder.HasMany(cart => cart.ShoppingCartItems) // һ�����ﳵ�ж�� ShoppingCartItems
                   .WithOne(sci => sci.Cart)
                   .HasForeignKey(sci => sci.Cart)
                   .OnDelete(DeleteBehavior.Cascade); // �����ﳵ��ɾ��ʱ���������������ҲӦ������ɾ�����Ա������ݸɾ���
        }
    }
}