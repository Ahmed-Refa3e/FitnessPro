using Core.Entities.ShopEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.ShopConfiguration
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasColumnType("VARCHAR").HasMaxLength(128).IsRequired();
            builder.Property(x => x.Description).HasColumnType("VARCHAR").HasMaxLength(256).IsRequired();
            builder.Property(x => x.Price).HasColumnType("decimal(9,2)").IsRequired();
            builder.HasMany(x => x.Categories).WithMany(x => x.Products);
            builder.HasOne(x => x.Shop).WithMany(x => x.Products).HasForeignKey(x => x.ShopId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
