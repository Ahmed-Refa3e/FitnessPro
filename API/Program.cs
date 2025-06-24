var builder = WebApplication.CreateBuilder(args);

// 1. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("http://localhost:4200", "https://gym-graduation-pr0ject.vercel.app");
    });
});

// 2. Stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// 3. Add controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Fitness Pro Web API",
        Description = "Fitness Pro - Web API for managing gym services"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    swagger.IncludeXmlComments(xmlPath);

    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token.\r\n\r\nExample: Bearer eyJhbGciOi..."
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
            Array.Empty<string>()
        }
    });
});


// 4. EF Core & Identity
builder.Services.AddDbContext<FitnessContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RemoteConnection"))//DefaultConnection RemoteConnection
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<FitnessContext>()
    .AddDefaultTokenProviders();

// 5. Application services & repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<GymPostRepository>();
builder.Services.AddScoped<ShopPostRepository>();
builder.Services.AddScoped<CoachPostRepository>();
builder.Services.AddScoped<IPostRepositoryFactory, PostRepositoryFactory>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<GoogleSettings>(builder.Configuration.GetSection("Authentication:Google"));

// 6. Authentication & Google
var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtSettings>()!;
var googleSettings = builder.Configuration.GetSection("Authentication:Google").Get<GoogleSettings>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.IssuerIP,
        ValidateAudience = true,
        ValidAudience = jwtSettings.AudienceIP,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecritKey ?? string.Empty))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ChatHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
})
.AddGoogle(options =>
{
    options.ClientId = googleSettings.ClientID;
    options.ClientSecret = googleSettings.ClientSecret;
});

// 7. Other repos & services
builder.Services.AddScoped<IGymRepository, GymRepository>();
builder.Services.AddScoped<GymRatingRepository>();
builder.Services.AddScoped<IGymService, GymService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IOnlineTrainingRepository, OnlineTrainingRepository>();
builder.Services.AddScoped<IOnlineTrainingSubscriptionRepository, OnlineTrainingSubscriptionRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IUserConnectionRepository, UserConnectionRepository>();
builder.Services.AddScoped<IChatService, ChatService>();

// 8. Azure BlobService registration 
var blobConn = builder.Configuration.GetConnectionString("BlobStorage");
builder.Services.AddScoped<IBlobService>(sp => new BlobService(blobConn!));

// 9. SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Seed roles & data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleInitializer.SeedRolesAsync(roleManager);

    try
    {
        await DataSeeder.Initialize(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

// 10. Middleware pipeline
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

app.MapHub<ChatHub>("/ChatHub");
app.MapControllers();

app.Run();
