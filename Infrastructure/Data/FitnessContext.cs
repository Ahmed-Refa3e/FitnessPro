﻿using Core.DTOs.PostDTO;
using Core.Entities.ChatEntites;
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
    public DbSet<Post>? Posts { get; set; }
    public DbSet<PostPictureUrl>? PictureUrls { get; set; }
    public DbSet<GymPost>? GymPosts { get; set; }
    public DbSet<ShopPost>? ShopPosts { get; set; }
    public DbSet<CoachPost>? CoachPosts { get; set; }
    public DbSet<Shop>? Shops { get; set; }
    public DbSet<Category>? categories { get; set; }
    public DbSet<Order>? orders { get; set; }
    public DbSet<OrderItem>? ordersItems { get; set; }
    public DbSet<Product>? products { get; set; }
    public DbSet<UserFollow>? userFollows { get; set; }
    public DbSet<GymFollow>? gymFollows { get; set; }
    public DbSet<ShopFollow>? ShopFollows { get; set; }
    public DbSet<CoachRating>? coachRatings { get; set; }
    public DbSet<Like>? likes { get; set; }
    public DbSet<PostLike>? postLikes { get; set; }
    public DbSet<CommentLike>? commentLikes { get; set; }
    public DbSet<Comment>? comments { get; set; }
    public DbSet<CommentComment>? commentComments { get; set; }
    public DbSet<PostComment>? postComments { get; set; }
    public DbSet<ChatMessage> messages { get; set; }
    public DbSet<UserConnection> connections { get; set; }

    //Schemas or View
    public DbSet<RawPostDTO> RawPostDTOs { get; set; }
    public DbSet<CountResult> countResults { get; set; }
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

        // OnlineTraining Relationships
        builder.Entity<OnlineTraining>()
            .HasOne(ot => ot.Coach)
            .WithMany(c => c.OnlineTrainings)
            .HasForeignKey(ot => ot.CoachID)
            .OnDelete(DeleteBehavior.NoAction);

        //OnlineTrainingSubscription Relationships

        builder.Entity<OnlineTrainingSubscription>()
                .HasOne(ots => ots.OnlineTraining)
                .WithMany(ot => ot.OnlineTrainingSubscriptions)
                .HasForeignKey(ots => ots.OnlineTrainingId)
                .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<OnlineTrainingSubscription>()
            .HasOne(ots => ots.Trainee)
            .WithMany(t => t.OnlineTrainingSubscriptions)
            .HasForeignKey(ots => ots.TraineeID)
            .OnDelete(DeleteBehavior.Restrict);

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
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserFollow>()
            .HasOne(e => e.FollowingUser)
            .WithMany(e => e.Followers)
            .HasForeignKey(e => e.FollowingId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<GymFollow>().HasKey(e => new { e.GymId, e.FollowerId });
        builder.Entity<GymFollow>()
            .HasOne(f => f.FollowerUser)
            .WithMany(u => u.FollowedGyms)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

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
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ShopFollow>()
            .HasOne(f => f.Shop)
            .WithMany(g => g.Followers)
            .HasForeignKey(f => f.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CoachRating>()
           .HasOne(gr => gr.Coach)
           .WithMany(g => g.Ratings)
           .HasForeignKey(gr => gr.CoachId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CoachRating>()
            .HasOne(gr => gr.Trainee)
            .WithMany()
            .HasForeignKey(gr => gr.TraineeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ChatMessage>()
            .HasOne(m => m.sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ChatMessage>()
            .HasOne(m => m.receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        //Index 
        builder.Entity<ShopFollow>().HasIndex(f => f.ShopId);
        builder
            .Entity<CountResult>()
            .HasNoKey()
            .ToView(null);
        builder
    .Entity<RawPostDTO>()
    .HasNoKey()
    .ToView(null);
    }
}
