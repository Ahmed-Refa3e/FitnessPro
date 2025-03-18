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
    internal class CommenCommentConfiguration : IEntityTypeConfiguration<CommentComment>
    {
        public void Configure(EntityTypeBuilder<CommentComment> builder)
        {
            builder.HasOne(x => x.Comment).WithMany(x => x.Comments).HasForeignKey(x => x.CommentId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
