using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.SetConfigs
{
    public class StoreConfig : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("STORE");

            // --- �����ͻ����������� ---
            builder.HasKey(s => s.StoreID);
            builder.Property(s => s.StoreID).HasColumnName("STOREID").ValueGeneratedOnAdd();

            builder.Property(s => s.StoreName).HasColumnName("STORENAME").IsRequired().HasMaxLength(50);

            builder.Property(s => s.StoreAddress).HasColumnName("STOREADDRESS").IsRequired().HasMaxLength(100);

            builder.Property(s => s.OpenTime).HasColumnName("OPENTIME").IsRequired();

            builder.Property(s => s.CloseTime).HasColumnName("CLOSETIME").IsRequired();

            builder.Property(s => s.AverageRating).HasColumnName("AVERAGERATING").HasColumnType("decimal(10,2)").HasDefaultValue(0.00m);

            builder.Property(s => s.MonthlySales).HasColumnName("MONTHLYSALES").IsRequired();

            builder.Property(s => s.StoreFeatures).HasColumnName("STOREFEATURES").IsRequired().HasMaxLength(500);

            builder.Property(s => s.StoreCreationTime).HasColumnName("STORECREATIONTIME").IsRequired();

            builder.Property(s => s.StoreState).HasColumnName("STORESTATE").IsRequired().HasConversion<string>().HasMaxLength(20);

            // ���Բ�ӳ�䵽���ݿ������
            builder.Ignore(s => s.IsOpen);
            builder.Ignore(s => s.BusinessHoursDisplay);

            // --- ����������� ---
            builder.Property(s => s.SellerID).HasColumnName("SELLERID").IsRequired();

            // ---------------------------------------------------------------
            // ��ϵ����
            // ---------------------------------------------------------------

            // ��ϵһ: Store -> Seller (һ��һ)
            builder.HasOne(s => s.Seller)
                   .WithOne(seller => seller.Store)
                   .HasForeignKey<Store>(s => s.SellerID)
                   .OnDelete(DeleteBehavior.Cascade); // ���̼ұ�ɾ��ʱ����ӵ�еĵ���ҲӦ������ɾ�����Ա�֤����һ���ԡ�

            // ��ϵ��: Store -> FoodOrder (һ�Զ�)
            builder.HasMany(s => s.FoodOrders)
                   .WithOne(fo => fo.Store)
                   .HasForeignKey(fo => fo.StoreID)
                   .OnDelete(DeleteBehavior.Restrict); // ������ɾ��һ��������ʷ�����ĵ��̣��Ա������׼�¼��

            // ��ϵ��: Store -> CouponManager (һ�Զ�)
            builder.HasMany(s => s.CouponManagers)
                   .WithOne(cm => cm.Store)
                   .HasForeignKey(cm => cm.StoreID)
                   .OnDelete(DeleteBehavior.Cascade); // �����̱�ɾ��ʱ�������õ��Ż�ȯ��ϢҲӦ������ɾ����

            // ��ϵ��: Store -> Menu (һ�Զ�)
            builder.HasMany(s => s.Menus)
                   .WithOne(m => m.Store)
                   .HasForeignKey(m => m.StoreID)
                   .OnDelete(DeleteBehavior.Cascade); // �����̱�ɾ��ʱ�������в˵�ҲӦ������ɾ������Ϊ�˵��ǵ��̵ĸ����

            // ��ϵ��: Store -> FavoriteItem (һ�Զ�)
            builder.HasMany(s => s.FavoriteItems)
                   .WithOne(fi => fi.Store)
                   .HasForeignKey(fi => fi.StoreID)
                   .OnDelete(DeleteBehavior.Cascade); // �����̱�ɾ��ʱ���û��ղؼ��й��ڴ˵��̵ļ�¼ҲӦ���Ƴ���

            // ��ϵ��: Store -> StoreViolationPenalty (һ�Զ�)
            builder.HasMany(s => s.StoreViolationPenalties)
                   .WithOne(svp => svp.Store)
                   .HasForeignKey(svp => svp.StoreID)
                   .OnDelete(DeleteBehavior.Restrict); // ������ɾ��һ����Υ�洦����¼�ĵ��̣��Ա�����Ҫ�Ĺ�����ʷ��

            // ��ϵ��: Store -> Comment (һ�Զ�)
            builder.HasMany(s => s.Comments)
                   .WithOne(c => c.Store)
                   .HasForeignKey(c => c.StoreID)
                   .OnDelete(DeleteBehavior.Restrict); // ������ɾ��һ�����û����۵ĵ��̣��Ա����û����ɵ����ݡ�

            // ��ϵ��: Store -> DeliveryTask (һ�Զ�)
            builder.HasMany(s => s.DeliveryTasks)
                   .WithOne(dt => dt.Store)
                   .HasForeignKey(dt => dt.StoreID)
                   .OnDelete(DeleteBehavior.SetNull); // ������̱�ɾ������ص����������¼����ɾ��������������̵Ĺ�����ΪNULL��
        }
    }
}
