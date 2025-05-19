using Core.Entities.PostEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.PostsConfiguration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content).HasMaxLength(512).IsRequired();
            builder.HasDiscriminator<string>("PostType").HasValue<CoachPost>("COH").HasValue<ShopPost>("SHP").HasValue<GymPost>("GYM");
            builder.Property("PostType").HasMaxLength(3);
            builder.HasIndex(x => x.CreatedAt);
            builder.ToTable("Posts");
        }
    }
}
