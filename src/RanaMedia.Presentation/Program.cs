using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RanaMedia.Application.Extensions;
using RanaMedia.Application.Profiles;
using RanaMedia.Contract.Services.Editorial;
using RanaMedia.Domain.Entities;
using RanaMedia.Persistence.Data;
using RanaMedia.Persistence.Data.Seed;
using RanaMedia.Persistence.Extension;
using RanaMedia.Presentation.ExceptionHandler;
using RanaMedia.Presentation.Infrastructure;
using Serilog;

namespace RanaMedia.Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var builder = WebApplication.CreateBuilder(args);

        // LOGGING (Serilog)
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog();

        // DATABASE (PostgreSQL + EF Core)
        var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection")
            ?? "Host=localhost;Port=5432;Database=rana_media_db;Username=postgres;Password=postgres";

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // CLEAN ARCHITECTURE LAYERS
        builder.Services.AddRepositories();
        builder.Services.AddApplicationServices(builder.Configuration);

        // PRESENTATION INFRASTRUCTURE
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IMediaStorageService, LocalFileStorageService>();
        builder.Services.AddScoped<ITurnstileVerifier, CloudflareTurnstileVerifier>();

        // CONTROLLERS & JSON
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOutputCache();

        // IDENTITY
        builder.Services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // JWT AUTHENTICATION
        var secretKey = builder.Configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey təyin edilməyib.");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        // SWAGGER WITH BEARER
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RanaMedia API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Bearer {token}"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // AUTOMAPPER
        builder.Services.AddAutoMapper(m => m.AddProfile(new CustomProfile()));

        // GLOBAL EXCEPTION HANDLERS (.NET 8 IExceptionHandler chain)
        builder.Services.AddExceptionHandler<NullExceptionHandler>();
        builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();
        builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
        builder.Services.AddExceptionHandler<InvalidCredentialsExceptionHandler>();
        builder.Services.AddExceptionHandler<ForbiddenExceptionHandler>();
        builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
        builder.Services.AddExceptionHandler<ConflictExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("cors", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Ensure uploads folder exists
        var webRoot = string.IsNullOrWhiteSpace(app.Environment.WebRootPath)
            ? Path.Combine(app.Environment.ContentRootPath, "wwwroot")
            : app.Environment.WebRootPath;
        Directory.CreateDirectory(Path.Combine(webRoot, "uploads"));

        // Startup migration & seed (with brief retry for containerized PostgreSQL boot)
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (db.Database.IsNpgsql())
            {
                var connected = false;
                for (var attempt = 1; attempt <= 5; attempt++)
                {
                    if (await db.Database.CanConnectAsync())
                    {
                        connected = true;
                        break;
                    }
                    Log.Information("PostgreSQL gözlənilir ({Attempt}/5)...", attempt);
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }

                if (connected)
                {
                    await db.Database.MigrateAsync();
                    await DataSeeder.SeedAsync(app.Services);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "PostgreSQL başlanğıc qoşulma/seed əməliyyatı atlandı.");
        }

        app.UseExceptionHandler();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors("cors");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseOutputCache();

        app.MapControllers();

        await app.RunAsync();
    }
}

