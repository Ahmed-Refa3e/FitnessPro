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
    public class PictureUrlConfiguration : IEntityTypeConfiguration<PictureUrl>
    {
        public void Configure(EntityTypeBuilder<PictureUrl> builder)
        {
            builder.Property(x => x.Url).IsRequired();
            builder.HasOne(x=>x.Post).WithMany(x=>x.PictureUrls).HasForeignKey(x=>x.PostId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
