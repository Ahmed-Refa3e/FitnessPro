using Core.Entities.PostEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.PostsConfiguration
{
    internal class CommenCommentConfiguration : IEntityTypeConfiguration<CommentComment>
    {
        public void Configure(EntityTypeBuilder<CommentComment> builder)
        {
            builder.HasOne(x => x.Comment).WithMany(x => x.Comments).HasForeignKey(x => x.CommentId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
