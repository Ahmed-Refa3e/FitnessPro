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
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content).HasMaxLength(1000).IsRequired();
            builder.HasDiscriminator<string>("PostType").HasValue<CoachPost>("COH").HasValue<ShopPost>("SHP").HasValue<GymPost>("GYM");
            builder.ToTable("Posts");
        }
    }
}
