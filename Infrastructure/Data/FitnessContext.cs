using Core.Entities;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;
public class FitnessContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{

    // DbSets
    public DbSet<ApplicationUser> applicationUsers { get; set; }
    public DbSet<Gym> Gyms { get; set; }
    public DbSet<GymSubscription> GymSubscriptions { get; set; }
    public DbSet<OnlineTraining> OnlineTrainings { get; set; }
    public DbSet<OnlineTrainingSubscription> OnlineTrainingSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Gym Relationships
        builder.Entity<Gym>()
            .HasOne(g => g.Owner)
            .WithOne(c => c.Gym)
            .HasForeignKey<Gym>(g => g.CoachID)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<GymSubscription>()
            .HasOne(gs => gs.Trainee)
            .WithMany(t => t.GymSubscriptions)
            .HasForeignKey(gs => gs.TraineeID)
            .OnDelete(DeleteBehavior.Cascade);

        // OnlineTraining Relationships
        builder.Entity<OnlineTraining>()
            .HasOne(ot => ot.Coach)
            .WithMany(c => c.OnlineTrainings)
            .HasForeignKey(ot => ot.CoachID)
            .OnDelete(DeleteBehavior.Cascade);

        // OnlineTrainingSubscription Relationships
        builder.Entity<OnlineTrainingSubscription>()
            .HasOne(ots => ots.Training)
            .WithMany()
            .HasForeignKey(ots => ots.TrainingID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OnlineTrainingSubscription>()
            .HasOne(ots => ots.Trainee)
            .WithMany(t => t.OnlineTrainingSubscriptions)
            .HasForeignKey(ots => ots.TraineeID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
