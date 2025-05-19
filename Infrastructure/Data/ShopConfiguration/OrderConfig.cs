using Core.Entities.ShopEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.ShopConfiguration
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TotalPrice).HasColumnType("decimal(12,2)").IsRequired();
            builder.HasOne(x => x.User).WithMany(x => x.Orders).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Shop).WithMany(x => x.Orders).HasForeignKey(x => x.ShopId).OnDelete(DeleteBehavior.SetNull);
            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.ShopId);
        }
    }
}
