using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using Infrastructure.Data;
using Infrastructure.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

public static class DataSeeder
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using (var context = serviceProvider.GetRequiredService<FitnessContext>())
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            await EnsureRoleExists(roleManager, "Coach");
            await EnsureRoleExists(roleManager, "Trainee");
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string jsonFilePath = Path.Combine(basePath, "SeedData", "seeding_data.json");

            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine("File not found: " + jsonFilePath);
                return;
            }

            var jsonData = File.ReadAllText(jsonFilePath);
            var seedData = JsonConvert.DeserializeObject<SeedingData>(jsonData);
            if (seedData == null) return;

            await SeedCoaches(userManager, seedData);

            await SeedTrainees(userManager, seedData);

            await SeedGyms(context, userManager, seedData);

            await SeedOnlineTrainings(context, userManager, seedData);

            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureRoleExists(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task SeedCoaches(UserManager<ApplicationUser> userManager, SeedingData seedData)
    {
        var existingCoaches = await userManager.GetUsersInRoleAsync("Coach");
        if (!existingCoaches.Any())
        {
            foreach (var coach in seedData.Coaches)
            {
                var newCoach = new Coach
                {
                    UserName = coach.Email,
                    Email = coach.Email,
                    FirstName = coach.FirstName,
                    LastName = coach.LastName,
                    DateOfBirth = coach.DateOfBirth,
                    Gender = coach.Gender,
                    Bio = coach.Bio,
                    JoinedDate = DateTime.Now,
                    EmailConfirmed = true,
                    ProfilePictureUrl = coach.ProfilePictureUrl,
                };

                var result = await userManager.CreateAsync(newCoach, "P@ssw0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newCoach, "Coach");
                }
            }
        }
    }

    private static async Task SeedTrainees(UserManager<ApplicationUser> userManager, SeedingData seedData)
    {
        var existingTrainees = await userManager.GetUsersInRoleAsync("Trainee");
        if (!existingTrainees.Any())
        {
            foreach (var trainee in seedData.Trainees)
            {
                var newTrainee = new Trainee
                {
                    UserName = trainee.Email,
                    Email = trainee.Email,
                    FirstName = trainee.FirstName,
                    LastName = trainee.LastName,
                    DateOfBirth = trainee.DateOfBirth,
                    Gender = trainee.Gender,
                    JoinedDate = DateTime.Now,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newTrainee, "P@ssw0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newTrainee, "Trainee");
                }
            }
        }
    }

    private static async Task SeedGyms(FitnessContext context, UserManager<ApplicationUser> userManager, SeedingData seedData)
    {
        var existingGyms = await context.Gyms.AnyAsync();
        if (!existingGyms)
        {
            var coaches = await userManager.GetUsersInRoleAsync("Coach");
            foreach (var gym in seedData.Gyms)
            {
                var coach = coaches.FirstOrDefault(c => c.Email == gym.CoachEmail);
                if (coach != null)
                {
                    await context.Gyms.AddAsync(new Gym
                    {
                        GymName = gym.GymName,
                        Address = gym.Address,
                        City = gym.City,
                        Governorate = gym.Governorate,
                        MonthlyPrice = gym.MonthlyPrice,
                        YearlyPrice = gym.YearlyPrice,
                        FortnightlyPrice = gym.FortnightlyPrice,
                        SessionPrice = gym.SessionPrice,
                        PhoneNumber = gym.PhoneNumber,
                        Description = gym.Description,
                        CoachID = coach.Id,
                        PictureUrl = gym.ProfilePictureUrl
                    });
                }
            }
        }
    }

    private static async Task SeedOnlineTrainings(FitnessContext context, UserManager<ApplicationUser> userManager, SeedingData seedData)
    {
        var existingTrainings = await context.OnlineTrainings.AnyAsync();
        if (!existingTrainings)
        {
            var coaches = await userManager.GetUsersInRoleAsync("Coach");
            foreach (var training in seedData.OnlineTrainings)
            {
                var coach = coaches.FirstOrDefault(c => c.Email == training.CoachEmail);
                if (coach != null)
                {
                    await context.OnlineTrainings.AddAsync(new OnlineTraining
                    {
                        Title = training.Title,
                        Description = training.Description,
                        TrainingType = training.TrainingType.ToString(), // Convert enum to string
                        Price = training.Price,
                        NoOfSessionsPerWeek = training.NoOfSessionsPerWeek,
                        DurationOfSession = training.DurationOfSession,
                        CoachID = coach.Id
                    });
                }
            }
        }
    }
}
