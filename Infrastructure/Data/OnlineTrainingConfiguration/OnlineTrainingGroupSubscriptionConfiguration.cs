using Core.Entities.OnlineTrainingEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Configuration
{
    internal class OnlineTrainingGroupSubscriptionConfiguration : IEntityTypeConfiguration<OnlineTrainingSubscription>
    {
        public void Configure(EntityTypeBuilder<OnlineTrainingSubscription> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.Trainee).WithMany(x=>x.OnlineTrainingSubscriptions).HasForeignKey(x=>x.TraineeID)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.OnlineTraining).WithMany(x => x.OnlineTrainingSubscriptions)
                .HasForeignKey(x => x.OnlineTrainingId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.Cost).HasColumnType("decimal(6,2)").IsRequired();
            builder.ToTable("OnlineTrainingSubscriptions");
        }
    }
}