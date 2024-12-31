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
    public class CoachPostConfiguration : IEntityTypeConfiguration<CoachPost>
    {
        public void Configure(EntityTypeBuilder<CoachPost> builder)
        {
            builder.HasOne(x => x.Coach).WithMany(x => x.Posts).HasForeignKey(x => x.CoachId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
