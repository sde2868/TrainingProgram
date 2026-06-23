using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.Json.Serialization;
using TraineeManagement.Services;
using TraineeManagement.Data;
using TraineeManagement.Middlewares;
using TraineeManagement.Models;
using TraineeManagement.Interfaces;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

// dotnet add package Swashbuckle.AspNetCore
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName!.Replace("+", "."));

    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT Token"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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

builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("Storage"));
builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileStorage"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddScoped<ITrainee, TraineeServices>();
builder.Services.AddScoped<IUser, UserServices>();
builder.Services.AddScoped<IMentor, MentorServices>();
builder.Services.AddScoped<ILearningTask, LearningTaskServices>();
builder.Services.AddScoped<ITaskAssignment, TaskAssignmentServices>();
builder.Services.AddScoped<ITaskSubmission, TaskSubmissionServices>();
builder.Services.AddScoped<IReview, ReviewServices>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageServices>();
builder.Services.AddScoped<ISubmissionFileService, SubmissionFileServices>();
builder.Services.AddScoped<ICacheService, RedisCacheServices>();
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Health checks: database and optional external-service checks
builder.Services.AddHealthChecks()
    .AddCheck<TraineeManagement.Services.HealthChecks.DatabaseHealthCheck>("database", tags: new[] { "ready" })
    .AddCheck<TraineeManagement.Services.HealthChecks.RedisHealthCheck>("redis", tags: new[] { "ready" })
    .AddCheck<TraineeManagement.Services.HealthChecks.ExternalServiceHealthCheck>("external-api", tags: new[] { "ready" });

// Typed HTTP client for external health checks with a short default timeout
builder.Services.AddHttpClient<TraineeManagement.Services.HealthChecks.ExternalServiceHealthCheck>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .ConfigureHttpClient((sp, client) =>
    {
        var cfg = sp.GetRequiredService<IConfiguration>();
        var seconds = int.TryParse(cfg["Health:ExternalTimeoutSeconds"], out var s) ? s : 5;
        client.Timeout = TimeSpan.FromSeconds(seconds);
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// builder.Services.AddDbContext<AppDbContext>(opt =>
//     opt.UseInMemoryDatabase("TraineeDb"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    )
            };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // app.UseExceptionHandler("/Error"); // Use when separate /Error page is needed when rendering views
    app.UseHsts();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// liveness (simple)
// Minimal API wrappers so Swagger/OpenAPI shows health endpoints
app.MapGet("/health/live", () => Results.Ok(new { status = "Alive", timestamp = DateTime.UtcNow }))
    .WithName("Liveness")
    .WithTags("Health")
    .Produces(200);

// readiness (run registered 'ready' checks)
app.MapGet("/health/ready", async (HealthCheckService hc, CancellationToken ct) =>
{
    var report = await hc.CheckHealthAsync(reg => reg.Tags.Contains("ready"), ct);
    var result = new
    {
        status = report.Status.ToString(),
        totalDuration = report.TotalDuration.TotalMilliseconds,
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            description = e.Value.Description,
            duration = e.Value.Duration.TotalMilliseconds
        })
    };

    return report.Status == HealthStatus.Healthy
        ? Results.Ok(result)
        : Results.Json(result, statusCode: StatusCodes.Status503ServiceUnavailable);
})
    .WithName("Readiness")
    .WithTags("Health")
    .Produces(200)
    .Produces(StatusCodes.Status503ServiceUnavailable);

app.MapControllers();

app.Run();