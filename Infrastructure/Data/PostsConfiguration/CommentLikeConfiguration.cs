using Core.Entities.PostEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.PostsConfiguration
{
    public class CommentLikeConfiguration : IEntityTypeConfiguration<CommentLike>
    {
        public void Configure(EntityTypeBuilder<CommentLike> builder)
        {
            builder.HasOne(x => x.Comment).WithMany(x => x.Likes).HasForeignKey(x => x.CommentId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
