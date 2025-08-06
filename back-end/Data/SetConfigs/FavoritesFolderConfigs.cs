using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEnd.Data.SetConfigs
{
    public class FavoritesFolderConfig : IEntityTypeConfiguration<FavoritesFolder>
    {
        public void Configure(EntityTypeBuilder<FavoritesFolder> builder)
        {
            builder.ToTable("FAVORITESFOLDER");

            builder.HasKey(ff => ff.FolderID);

            builder.Property(ff => ff.FolderID).HasColumnName("FOLDERID").ValueGeneratedOnAdd();

            builder.Property(ff => ff.FolderName).HasColumnName("FOLDERNAME").IsRequired().HasMaxLength(50);

            builder.Property(ff => ff.CustomerID).HasColumnName("CUSTOMERID").IsRequired();

            // ---------------------------------------------------------------
            // ��ϵ����
            // ---------------------------------------------------------------

            // ��ϵһ: FavoritesFolder -> Customer (���һ)
            // Customer ������ FavoritesFolders �������ԣ�Ӧ��ȷָ��
            builder.HasOne(f => f.Customer)
                   .WithMany(c => c.FavoritesFolders) // ��ȷָ�� Customer �˵ķ��򵼺�����
                   .HasForeignKey(f => f.CustomerID)
                   .OnDelete(DeleteBehavior.Cascade); // ���˿ͱ�ɾ��ʱ���������ղؼ�ҲӦ������ɾ��

            // ��ϵ��: FavoritesFolder -> FavoriteItem (һ�Զ�)
            builder.HasMany(f => f.FavoriteItems)
                   .WithOne(fi => fi.Folder) // ��ȷָ�� FavoriteItem �˵ķ��򵼺�����
                   .HasForeignKey(fi => fi.FolderID)
                   .OnDelete(DeleteBehavior.Cascade); // ���ղؼб�ɾ��ʱ��������������ղ��Ӧ������ɾ��
        }
    }
}