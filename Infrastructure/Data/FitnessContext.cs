using Core.Entities.FollowEntities;
using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using Core.Entities.PostEntities;
using Core.Entities.ShopEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;
public class FitnessContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{

    // DbSets
    public DbSet<ApplicationUser> applicationUsers { get; set; }
    public DbSet<Gym>? Gyms { get; set; }
    public DbSet<GymSubscription>? GymSubscriptions { get; set; }
    public DbSet<GymRating>? GymRatings { get; set; }
    public DbSet<OnlineTraining>? OnlineTrainings { get; set; }
    public DbSet<OnlineTrainingSubscription>? OnlineTrainingSubscriptions { get; set; }
    public DbSet<OnlineTrainingGroup>? OnlineTrainingGroups { get; set; }
    public DbSet<OnlineTrainingPrivate>? OnlineTrainingPrivates { get; set; }
    public DbSet<Post>? Posts { get; set; }
    public DbSet<PostPictureUrl>? PictureUrls { get; set; } 
    public DbSet<GymPost>? GymPosts { get; set; }
    public DbSet<ShopPost>? ShopPosts { get; set; }
    public DbSet<CoachPost>? CoachPosts { get; set; }
    public DbSet<Shop> Shops { get; set; }
    public DbSet<UserFollow> userFollows { get; set; }
    public DbSet<GymFollow> gymFollows { get; set; }
    public DbSet<ShopFollow> ShopFollows { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(FitnessContext).Assembly);
        #region relationships
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

       /* // OnlineTraining Relationships
        builder.Entity<OnlineTraining>()
            .HasOne(ot => ot.Coach)
            .WithMany(c => c.OnlineTrainings)
            .HasForeignKey(ot => ot.CoachID)
            .OnDelete(DeleteBehavior.Cascade);

        // OnlineTrainingSubscription Relationships
        /*builder.Entity<OnlineTrainingSubscription>()
            .HasOne(ots => ots.Training)
            .WithMany()
            .HasForeignKey(ots => ots.TrainingID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OnlineTrainingSubscription>()
            .HasOne(ots => ots.Trainee)
            .WithMany(t => t.OnlineTrainingSubscriptions)
            .HasForeignKey(ots => ots.TraineeID)
            .OnDelete(DeleteBehavior.Cascade);*/

        // GymRating relationships
        builder.Entity<GymRating>()
            .HasOne(gr => gr.Gym)
            .WithMany(g => g.Ratings)
            .HasForeignKey(gr => gr.GymID)
            .OnDelete(DeleteBehavior.Cascade); // If a gym is deleted, all associated ratings are also deleted

        builder.Entity<GymRating>()
            .HasOne(gr => gr.Trainee)
            .WithMany()
            .HasForeignKey(gr => gr.TraineeID)
            .OnDelete(DeleteBehavior.Cascade); // if a trainee is deleted, all associated ratings are also deleted

        builder.Entity<UserFollow>().HasKey(e => new { e.FollowingId, e.FollowerId });

        builder.Entity<UserFollow>()
            .HasOne(e => e.FollowerUser)
            .WithMany(e => e.Following)
            .HasForeignKey(e => e.FollowerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<UserFollow>()
            .HasOne(e => e.FollowingUser)
            .WithMany(e => e.Followers)
            .HasForeignKey(e => e.FollowingId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<GymFollow>().HasKey(e => new {e.GymId, e.FollowerId});
        builder.Entity<GymFollow>()
            .HasOne(f => f.FollowerUser)
            .WithMany(u => u.FollowedGyms)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<GymFollow>()
            .HasOne(f => f.Gym)
            .WithMany(g => g.Followers)
            .HasForeignKey(f => f.GymId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ShopFollow>().HasKey(e => new { e.ShopId, e.FollowerId });
        builder.Entity<ShopFollow>()
            .HasOne(f => f.FollowerUser)
            .WithMany(u => u.FollowedShops)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ShopFollow>()
            .HasOne(f => f.Shop)
            .WithMany(g => g.Followers)
            .HasForeignKey(f => f.ShopId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion

        #region seeding data
        // Seeding Coach data (with all inherited properties from ApplicationUser)
        builder.Entity<Coach>().HasData(
            new Coach
            {
                Id = "coach1", 
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                DateOfBirth = new DateTime(1985, 5, 24),
                Email = "johndoe@example.com",
                UserName = "johndoe",
                NormalizedUserName = "JOHNDOE",
                NormalizedEmail = "JOHNDOE@EXAMPLE.COM",
                EmailConfirmed = false,
                PhoneNumber = "0123456789",
                AvailableForOnlineTraining = true
            },
            new Coach
            {
                Id = "coach2",
                FirstName = "Jane",
                LastName = "Smith",
                Gender = "Female",
                DateOfBirth = new DateTime(1990, 8, 14),
                Email = "janesmith@example.com",
                UserName = "janesmith",
                NormalizedUserName = "JANESMITH",
                NormalizedEmail = "JANESMITH@EXAMPLE.COM",
                EmailConfirmed = false,
                PhoneNumber = "0987654321",
                AvailableForOnlineTraining = false
            }
        );

        // Seeding Gym data
        builder.Entity<Gym>().HasData(
            new Gym
            {
                GymID = 1,
                GymName = "Downtown Fitness",
                Address = "123 Main St",
                City = "Tanta",
                Governorate = "Gharbia",
                MonthlyPrice = 50,
                YearlyPrice = 500,
                FortnightlyPrice = 30,
                SessionPrice = 15,
                PhoneNumber = "0123456789",
                Description = "A top-tier gym with all the modern equipment you need.",
                CoachID = "coach1" // Linking to seeded coach
            },
            new Gym
            {
                GymID = 2,
                GymName = "Sunset Wellness",
                Address = "456 Sunset Blvd",
                City = "Zefta",
                Governorate = "Gharbia",
                MonthlyPrice = 40,
                YearlyPrice = 450,
                FortnightlyPrice = 25,
                SessionPrice = 12,
                PhoneNumber = "0987654321",
                Description = "A wellness center focused on body and mind fitness.",
                CoachID = "coach2" // Linking to seeded coach
            }
        );
        #endregion
    }
}
