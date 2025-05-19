using Core.Entities.PostEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.PostsConfiguration
{
    public class PostPictureUrlConfiguration : IEntityTypeConfiguration<PostPictureUrl>
    {
        public void Configure(EntityTypeBuilder<PostPictureUrl> builder)
        {
            builder.Property(x => x.Url).IsRequired();
            builder.HasOne(x => x.Post).WithMany(x => x.PictureUrls).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(x => x.PostId);
            builder.ToTable("PostPictureUrl");
        }
    }
}
