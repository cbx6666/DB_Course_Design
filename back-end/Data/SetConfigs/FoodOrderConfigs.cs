using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.SetConfigs
{
    public class FoodOrderConfig : IEntityTypeConfiguration<FoodOrder>
    {
        public void Configure(EntityTypeBuilder<FoodOrder> builder)
        {
            builder.ToTable("FOODORDER");

            builder.HasKey(fo => fo.OrderID);
            builder.Property(fo => fo.OrderID).HasColumnName("ORDERID").ValueGeneratedOnAdd();

            builder.Property(fo => fo.PaymentTime).HasColumnName("PAYMENTTIME").IsRequired();

            builder.Property(fo => fo.Remarks).HasColumnName("REMARKS").HasMaxLength(255);

            builder.Property(fo => fo.Rating).HasColumnName("RATING").HasColumnType("decimal(2,1)");

            builder.Property(fo => fo.RatingComment).HasColumnName("RATINGCOMMENT").HasMaxLength(500);

            builder.Property(fo => fo.RatingTime).HasColumnName("RATINGTIME");

            builder.Property(fo => fo.CustomerID).HasColumnName("CUSTOMERID").IsRequired();

            builder.Property(fo => fo.CartID).HasColumnName("CARTID").IsRequired();

            builder.Property(fo => fo.StoreID).HasColumnName("STOREID").IsRequired();

            // ---------------------------------------------------------------
            // ���ù�ϵ
            // ---------------------------------------------------------------

            // ��ϵһ: FoodOrder -> Customer (���һ)
            builder.HasOne(e => e.Customer)
                   .WithMany(c => c.FoodOrders)
                   .HasForeignKey(e => e.CustomerID)
                   .OnDelete(DeleteBehavior.Restrict); // ��ֹɾ�����ж����Ĺ˿�

            // ��ϵ��: FoodOrder -> ShoppingCart (һ��һ)
            // һ��������Ӧһ�����ﳵ��һ�����ﳵ�����Ҳֻ����һ������
            builder.HasOne(e => e.Cart)
                   .WithOne(c => c.Order)
                   .HasForeignKey<FoodOrder>(e => e.CartID)
                   .OnDelete(DeleteBehavior.Restrict); // ��ֹɾ�������ɶ����Ĺ��ﳵ

            // ��ϵ��: FoodOrder -> Store (���һ)
            builder.HasOne(e => e.Store)
                   .WithMany(s => s.FoodOrders)
                   .HasForeignKey(e => e.StoreID)
                   .OnDelete(DeleteBehavior.Restrict); // ��ֹɾ�����ж������̵�

            // ��ϵ��: FoodOrder -> Coupon (һ�Զ�)
            builder.HasMany(e => e.Coupons)
                   .WithOne(c => c.Order)
                   .HasForeignKey(c => c.OrderID)
                   .OnDelete(DeleteBehavior.SetNull); // ����ɾ��ʱ���������Ż�ȯ��Ϊδʹ�ã�������ɾ���Ż�ȯ����

            // ��ϵ��: FoodOrder -> AfterSaleApplication (һ�Զ�)
            builder.HasMany(e => e.AfterSaleApplications)
                   .WithOne(a => a.Order)
                   .HasForeignKey(a => a.OrderID)
                   .OnDelete(DeleteBehavior.Cascade); // ��������ɾ��ʱ���������ۺ����붼Ӧ������ɾ��
        }
    }
}