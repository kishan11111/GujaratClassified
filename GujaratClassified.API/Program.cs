//// Program.cs
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;
//using GujaratClassified.API.DAL.Interfaces;
//using GujaratClassified.API.DAL;
//using GujaratClassified.API.DAL.Repositories;
//using GujaratClassified.API.Services.Interfaces;
//using GujaratClassified.API.Services.Implementations;
//using GujaratClassified.API.Helpers;
//using GujaratClassified.API.Models.Common;
//using Serilog;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();

//// Configure JWT Settings
//builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
//var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

//// Configure JWT Authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings.Issuer,
//        ValidAudience = jwtSettings.Audience,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
//        ClockSkew = TimeSpan.Zero
//    };
//});

//// Register Database Services
//builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

//// Register Repositories
//builder.Services.AddScoped<IAdminRepository, AdminRepository>();
//builder.Services.AddScoped<ILocationRepository, LocationRepository>();
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IOTPRepository, OTPRepository>();
//builder.Services.AddScoped<IPostRepository, PostRepository>();
//builder.Services.AddScoped<IPostImageRepository, PostImageRepository>();
//builder.Services.AddScoped<IPostVideoRepository, PostVideoRepository>();
//builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
//builder.Services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
//builder.Services.AddScoped<IAdInquiryRepository, AdInquiryRepository>();
//builder.Services.AddScoped<IPostLikeRepository, PostLikeRepository>();
//builder.Services.AddScoped<IPostCommentRepository, PostCommentRepository>();
//builder.Services.AddScoped<IAgriFieldRepository, AgriFieldRepository>();
//builder.Services.AddScoped<IAgriFieldImageRepository, AgriFieldImageRepository>();
//builder.Services.AddScoped<IAgriFieldVideoRepository, AgriFieldVideoRepository>();
//builder.Services.AddScoped<IAgriFieldCommentRepository, AgriFieldCommentRepository>();
//builder.Services.AddScoped<IAgriFieldLikeRepository, AgriFieldLikeRepository>();
//builder.Services.AddScoped<IAgriFieldFollowRepository, AgriFieldFollowRepository>();
//builder.Services.AddScoped<IFarmerProfileRepository, FarmerProfileRepository>();
//builder.Services.AddScoped<ILocalCardRepository, LocalCardRepository>();
//builder.Services.AddScoped<ILocalCardCategoryRepository, LocalCardCategoryRepository>();
//builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

//// Register Services
//builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
//builder.Services.AddScoped<ILocationMasterService, LocationMasterService>();
//builder.Services.AddScoped<ICategoryMasterService, CategoryMasterService>();
//builder.Services.AddScoped<IUserAuthService, UserAuthService>();
//builder.Services.AddScoped<IOTPService, OTPService>();
//builder.Services.AddScoped<IUserPostService, UserPostService>();
//builder.Services.AddScoped<IFileUploadService, FileUploadService>();
//builder.Services.AddScoped<IBrowseService, BrowseService>();
//builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
//builder.Services.AddScoped<IPublicAdvertisementService, PublicAdvertisementService>();
//builder.Services.AddScoped<IPostLikeService, PostLikeService>();
//builder.Services.AddScoped<IPostCommentService, PostCommentService>();
//builder.Services.AddScoped<IAgriFieldService, AgriFieldService>();
//builder.Services.AddScoped<IFarmerProfileService, FarmerProfileService>();
//builder.Services.AddScoped<ILocalCardService, LocalCardService>();
//builder.Services.AddScoped<ILocalCardCategoryService, LocalCardCategoryService>();
//builder.Services.AddScoped<INotificationService, NotificationService>();

//// Register Helpers
//builder.Services.AddScoped<IJwtHelper, JwtHelper>();

//// Configure Swagger/OpenAPI
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//    {
//        Title = "Gujarat Classified API",
//        Version = "v1",
//        Description = "API for Gujarat Classified Portal"
//    });

//    // Add JWT Authentication to Swagger
//    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
//        Name = "Authorization",
//        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
//    {
//        {
//            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//            {
//                Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                {
//                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                },
//                Scheme = "oauth2",
//                Name = "Bearer",
//                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//            },
//            new List<string>()
//        }
//    });
//});

//// Add CORS
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(builder =>
//    {
//        builder.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//    });
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
////if (app.Environment.IsDevelopment())
////{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gujarat Classified API v1");
//        c.RoutePrefix = "swagger"; // Swagger available at /swagger
//    });
////}

//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Debug()
//    .WriteTo.Console()
//    .WriteTo.File(
//        path: "Logs/log-.txt",          // folder where logs are saved
//        rollingInterval: RollingInterval.Day, // new file every day
//        retainedFileCountLimit: 30,     // keep last 30 days
//        fileSizeLimitBytes: 10_000_000, // 10 MB max per file
//        rollOnFileSizeLimit: true
//    )
//    .CreateLogger();

//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    // Auto-detects base path (works with IIS virtual directory)
//    c.SwaggerEndpoint($"{(string.IsNullOrEmpty(c.RoutePrefix) ? "." : "..")}/swagger/v1/swagger.json",
//                      "Gujarat Classified API v1");

//    // Keep Swagger UI at /swagger
//    c.RoutePrefix = "swagger";
//});

//app.UseHttpsRedirection();

//// Add static file serving for uploads
//app.UseStaticFiles();

//app.UseCors();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();

// Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.DAL;
using GujaratClassified.API.DAL.Repositories;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Services.Implementations;
using GujaratClassified.API.Helpers;
using GujaratClassified.API.Models.Common;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging to File
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add custom file logging
builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>(provider =>
    new FileLoggerProvider("Logs"));

// Add services to the container.
builder.Services.AddControllers();

// Configure JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };
});

// Register Database Services
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// Register Repositories
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOTPRepository, OTPRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostImageRepository, PostImageRepository>();
builder.Services.AddScoped<IPostVideoRepository, PostVideoRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
builder.Services.AddScoped<IAdInquiryRepository, AdInquiryRepository>();
builder.Services.AddScoped<IPostLikeRepository, PostLikeRepository>();
builder.Services.AddScoped<IPostCommentRepository, PostCommentRepository>();
builder.Services.AddScoped<IAgriFieldRepository, AgriFieldRepository>();
builder.Services.AddScoped<IAgriFieldImageRepository, AgriFieldImageRepository>();
builder.Services.AddScoped<IAgriFieldVideoRepository, AgriFieldVideoRepository>();
builder.Services.AddScoped<IAgriFieldCommentRepository, AgriFieldCommentRepository>();
builder.Services.AddScoped<IAgriFieldLikeRepository, AgriFieldLikeRepository>();
builder.Services.AddScoped<IAgriFieldFollowRepository, AgriFieldFollowRepository>();
builder.Services.AddScoped<IFarmerProfileRepository, FarmerProfileRepository>();
builder.Services.AddScoped<ILocalCardRepository, LocalCardRepository>();
builder.Services.AddScoped<ILocalCardCategoryRepository, LocalCardCategoryRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();

// Register Services
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<ILocationMasterService, LocationMasterService>();
builder.Services.AddScoped<ICategoryMasterService, CategoryMasterService>();
builder.Services.AddScoped<IUserAuthService, UserAuthService>();
builder.Services.AddScoped<IOTPService, OTPService>();
builder.Services.AddScoped<IUserPostService, UserPostService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IBrowseService, BrowseService>();
builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
builder.Services.AddScoped<IPublicAdvertisementService, PublicAdvertisementService>();
builder.Services.AddScoped<IPostLikeService, PostLikeService>();
builder.Services.AddScoped<IPostCommentService, PostCommentService>();
builder.Services.AddScoped<IAgriFieldService, AgriFieldService>();
builder.Services.AddScoped<IFarmerProfileService, FarmerProfileService>();
builder.Services.AddScoped<ILocalCardService, LocalCardService>();
builder.Services.AddScoped<ILocalCardCategoryService, LocalCardCategoryService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IBlogService, BlogService>();
// Register Helpers
builder.Services.AddScoped<IJwtHelper, JwtHelper>();


// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Gujarat Classified API",
        Version = "v1",
        Description = "API for Gujarat Classified Portal"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseStatusCodePages(async context =>
{
    // This ensures your custom error responses are returned as-is
    context.HttpContext.Response.ContentType = "application/json";
});
// Test log
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Gujarat Classified API is starting...");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gujarat Classified API v1");
    c.RoutePrefix = "swagger";
});

// IMPORTANT: Comment out HTTPS redirection for IIS
// app.UseHttpsRedirection();

// Add static file serving for uploads
app.UseStaticFiles();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

logger.LogInformation("Gujarat Classified API started successfully");

app.Run();

// Custom File Logger Implementation
public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logPath;
    private readonly object _lock = new object();

    public FileLoggerProvider(string logPath)
    {
        _logPath = logPath;
        if (!Directory.Exists(_logPath))
        {
            Directory.CreateDirectory(_logPath);
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, _logPath, _lock);
    }

    public void Dispose() { }
}

public class FileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly string _logPath;
    private readonly object _lock;

    public FileLogger(string categoryName, string logPath, object lockObj)
    {
        _categoryName = categoryName;
        _logPath = logPath;
        _lock = lockObj;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var logFileName = Path.Combine(_logPath, $"log-{DateTime.Now:yyyyMMdd}.txt");
        var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_categoryName} - {formatter(state, exception)}";

        if (exception != null)
        {
            logMessage += Environment.NewLine + exception.ToString();
        }

        lock (_lock)
        {
            try
            {
                File.AppendAllText(logFileName, logMessage + Environment.NewLine);
            }
            catch
            {
                // Silently fail if can't write to log file
            }
        }
    }
}