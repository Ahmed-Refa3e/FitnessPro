using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Repositories.PostRepositories;
using Core.Interfaces.Services;
using Infrastructure.Repositories;
using Services;
using System.Security.Claims;
using Infrastructure.Repositories.PostRepositoy;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Infrastructure.Repositories.OnlineTrainingRepositories;
using Core.Interfaces.Repositories.ShopRepositories;
using Infrastructure.Repositories.IShopRepositories;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation    
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ASP.NET 8 Web API",
        Description = "Store"
    });
    // To Enable authorization using Swagger (JWT)    
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                    });
});

builder.Services.AddDbContext<FitnessContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<FitnessContext>().AddDefaultTokenProviders(); ;

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostRepresentationRepository, PostRepresentationRepository>();
builder.Services.AddScoped<IShopRepository,ShopRepository>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings?.IssuerIP,
        ValidateAudience = true,
        ValidAudience = jwtSettings?.AudienceIP,
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings?.SecritKey ?? string.Empty))

    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userRepository.GetAsync(e => e.Id == userId, includeProperties: "refreshTokens");

            if (user == null || user.refreshTokens == null || user.refreshTokens.All(t => t.Revoked != null))
            {
                context.Fail("Token has been revoked.");
            }
        }
    };
});

builder.Services.AddScoped<IGymRepository, GymRepository>();
builder.Services.AddScoped<IGymService, GymService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IOnlineTrainingGroupRepository, OnlineTrainingGroupRepository>();
builder.Services.AddScoped<IOnlineTrainingPrivateRepository, OnlineTrainingPrivateRepository>();
builder.Services.AddScoped<IOnlineTrainingSubscriptionRepository, OnlineTrainingSubscriptionRepository>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await RoleInitializer.SeedRolesAsync(roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


//Apply any pending migrations
/*try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<FitnessContext>();
    await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}*/

app.Run();
