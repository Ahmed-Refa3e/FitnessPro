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
            builder.Property(x => x.Price).HasPrecision(9, 2).IsRequired();
            builder.Property(x => x.OfferPrice).HasPrecision(9, 2);
            //builder.HasMany(x => x.Categories).WithMany(x => x.Products);
            builder.HasMany(p => p.Categories).WithMany(c => c.Products).UsingEntity<Dictionary<string, object>>(
                    "ProductCategories",
                    pc => pc.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                    pc => pc.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                    pc =>
                    {
                        pc.HasIndex(new[] { "CategoryId", "ProductId" });
                        pc.ToTable("ProductCategories");
                    });
            builder.HasOne(x => x.Shop).WithMany(x => x.Products).HasForeignKey(x => x.ShopId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(c => c.ShopId);
            builder.ToTable("Products");
        }
    }
}
