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
    public class OnlineTrainingConfiguration : IEntityTypeConfiguration<OnlineTraining>
    {
        public void Configure(EntityTypeBuilder<OnlineTraining> builder)
        {
           // builder.HasDiscriminator<string>("TrainingType").HasValue<OnlineTrainingGroup>("GROUP").HasValue<OnlineTrainingPrivate>("PRIVT");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(50);
            builder.Property(x=>x.Description).HasMaxLength(250);
            builder.Property(x => x.Price).HasColumnType("decimal(6,2)").IsRequired();
           // builder.Property(x => x.OfferPrice).HasColumnType("decimal(6,2)");
            builder.HasOne(x => x.Coach).WithMany(x => x.OnlineTrainings).HasForeignKey(x => x.CoachID).OnDelete(DeleteBehavior.Cascade);
            builder.ToTable("OnlineTrainings");
        }
    }
}
