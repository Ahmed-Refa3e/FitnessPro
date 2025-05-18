using Core.Entities.PostEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.PostsConfiguration
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasOne(x => x.User).WithMany(x => x.Likes).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
            builder.HasDiscriminator<string>("LikeType").HasValue<PostLike>("POST").HasValue<CommentLike>("CMNT");
            builder.HasIndex(x => x.UserId);
            builder.Property("LikeType").HasMaxLength(4);
        }
    }
}
