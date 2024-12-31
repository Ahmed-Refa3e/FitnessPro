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
    public class GymPostConfiguration : IEntityTypeConfiguration<GymPost>
    {
        public void Configure(EntityTypeBuilder<GymPost> builder)
        {
            builder.HasOne(x => x.Gym).WithMany(x => x.Posts).HasForeignKey(x => x.GymId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
