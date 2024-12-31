using Core.Entities.ShopEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.ShopConfiguration
{
    internal class ShopConfiguration : IEntityTypeConfiguration<Shop>
    {
        public void Configure(EntityTypeBuilder<Shop> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(40).IsRequired();
            builder.Property(x=>x.Address).HasMaxLength(50).IsRequired();
            builder.Property(x=>x.City).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Governorate).HasMaxLength(20).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(15);
            builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
            builder.HasOne(x=>x.Owner).WithMany(x=>x.Shops).HasForeignKey(x=>x.CoachID);
            builder.ToTable("Shops");
        }
    }
}
