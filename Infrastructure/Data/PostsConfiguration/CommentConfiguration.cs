using Core.Entities.PostEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.PostsConfiguration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasOne(x => x.User).WithMany(x => x.Comments).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
            builder.HasDiscriminator<string>("CommentType").HasValue<CommentComment>("COM").HasValue<PostComment>("Pos");
            builder.Property("CommentType").HasMaxLength(3);
            builder.HasIndex(x => x.UserId);
        }
    }
}
