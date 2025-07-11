using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using Core.Entities.PostEntities;
using Core.Entities.ShopEntities;
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

            await SeedCoaches(userManager, seedData, context);

            await SeedTrainees(userManager, seedData, context);

            await SeedGyms(context, userManager, seedData);

            await SeedOnlineTrainings(context, userManager, seedData);

            await SeedShops(context, userManager, seedData);

            await SeedProducts(context, seedData);

            await SeedPosts(context, seedData);

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

    private static async Task SeedCoaches(UserManager<ApplicationUser> userManager, SeedingData seedData, FitnessContext context)
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
            await context.SaveChangesAsync();
        }

    }

    private static async Task SeedTrainees(UserManager<ApplicationUser> userManager, SeedingData seedData, FitnessContext context)
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
            await context.SaveChangesAsync();
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
            await context.SaveChangesAsync();
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
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedShops(FitnessContext context, UserManager<ApplicationUser> userManager, SeedingData seedData)
    {
        var existingShops = await context.Shops.AnyAsync();
        if (!existingShops)
        {
            var coaches = await userManager.GetUsersInRoleAsync("Coach");

            foreach (var shop in seedData.Shops)
            {
                var coach = coaches.FirstOrDefault(c => c.Email == shop.CoachEmail);
                if (coach != null)
                {
                    await context.Shops.AddAsync(new Shop
                    {
                        Name = shop.ShopName,
                        Address = shop.Address,
                        City = shop.City,
                        Governorate = shop.Governorate,
                        PhoneNumber = shop.PhoneNumber,
                        Description = shop.Description,
                        OwnerID = coach.Id,
                        PictureUrl = shop.PictureUrl
                    });
                }
            }
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedProducts(FitnessContext context, SeedingData seedData)
    {
        var existingProducts = await context.products.AnyAsync();
        if (!existingProducts)
        {
            var shops = await context.Shops.ToListAsync();
            var allCategories = await context.categories.ToListAsync(); 

            foreach (var product in seedData.Products)
            {
                var shop = shops.FirstOrDefault(s => s.Name == product.ShopName);
                if (shop == null) continue;

                var productCategories = new List<Category>();
                foreach (var catName in product.Categories)
                {
                    var existingCat = allCategories.FirstOrDefault(c => c.Name.Equals(catName, StringComparison.OrdinalIgnoreCase));
                    if (existingCat != null)
                    {
                        productCategories.Add(existingCat);
                    }
                    else
                    {
                        var newCat = new Category { Name = catName };
                        context.categories.Add(newCat);
                        productCategories.Add(newCat);
                        allCategories.Add(newCat); 
                    }
                }

                var newProduct = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    OfferPrice = product.OfferPrice,
                    ImagePath = product.ImagePath,
                    Quantity = product.Quantity,
                    ShopId = shop.Id,
                    Categories = productCategories
                };

                await context.products.AddAsync(newProduct);
            }

            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedPosts(FitnessContext context, SeedingData seedData)
    {
        var existingPosts = await context.Posts.AnyAsync();
        if (existingPosts) return;

        var coaches = await context.Users
            .Where(u => u is Coach)
            .Cast<Coach>()
            .ToListAsync();

        var gyms = await context.Gyms.ToListAsync();
        var shops = await context.Shops.ToListAsync();

        foreach (var post in seedData.Posts)
        {
            var basePost = new Post
            {
                Content = post.Content,
                CreatedAt = DateTime.UtcNow,
                PictureUrls = post.PictureUrls?.Select(url => new PostPictureUrl { Url = url }).ToList()
            };

            switch (post.Type.ToUpper())
            {
                case "COACH":
                    var coach = coaches.FirstOrDefault(c => c.Email == post.EmailOrName);
                    if (coach != null)
                    {
                        await context.CoachPosts.AddAsync(new CoachPost
                        {
                            Content = basePost.Content,
                            CreatedAt = basePost.CreatedAt,
                            PictureUrls = basePost.PictureUrls,
                            CoachId = coach.Id
                        });
                    }
                    break;

                case "GYM":
                    var gym = gyms.FirstOrDefault(g => g.GymName == post.EmailOrName);
                    if (gym != null)
                    {
                        await context.GymPosts.AddAsync(new GymPost
                        {
                            Content = basePost.Content,
                            CreatedAt = basePost.CreatedAt,
                            PictureUrls = basePost.PictureUrls,
                            GymId = gym.GymID
                        });
                    }
                    break;

                case "SHOP":
                    var shop = shops.FirstOrDefault(s => s.Name == post.EmailOrName);
                    if (shop != null)
                    {
                        await context.ShopPosts.AddAsync(new ShopPost
                        {
                            Content = basePost.Content,
                            CreatedAt = basePost.CreatedAt,
                            PictureUrls = basePost.PictureUrls,
                            ShopId = shop.Id
                        });
                    }
                    break;
            }
            await context.SaveChangesAsync();
        }
    }
}
