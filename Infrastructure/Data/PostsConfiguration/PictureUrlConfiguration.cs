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
    public class PictureUrlConfiguration : IEntityTypeConfiguration<PostPictureUrl>
    {
        public void Configure(EntityTypeBuilder<PostPictureUrl> builder)
        {
            builder.Property(x => x.Url).IsRequired();
            builder.HasOne(x=>x.Post).WithMany(x=>x.PictureUrls).HasForeignKey(x=>x.PostId).OnDelete(DeleteBehavior.Cascade);
            builder.ToTable("PostPictureUrl");
        }
    }
}
