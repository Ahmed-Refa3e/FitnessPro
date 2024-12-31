using Core.Entities.PostEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.PostsConfiguration
{
    public class ShopPostConfiguration : IEntityTypeConfiguration<ShopPost>
    {
        public void Configure(EntityTypeBuilder<ShopPost> builder)
        {
            builder.HasOne(x => x.Shop).WithMany(x => x.Posts).HasForeignKey(x => x.ShopId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
