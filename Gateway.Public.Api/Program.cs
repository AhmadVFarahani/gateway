using Gateway.Application.BackgroundServices;
using Gateway.Application.Implementations;
using Gateway.Application.Implementations.Cache;
using Gateway.Application.Interfaces;
using Gateway.Application.Interfaces.Cache;
using Gateway.Application.Usage;
using Gateway.Application.Yarp;
using Gateway.Domain.Interfaces;
using Gateway.Infrastructure.Repositories;
using Gateway.Persistence;
using Gateway.Public.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);

#region 🔹 Database (EF Core)
builder.Services.AddDbContext<GatewayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region 🔹 Repositories
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IScopeRepository, ScopeRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IAccessPolicyRepository, AccessPolicyRepository>();
builder.Services.AddScoped<IRouteScopeRepository, RouteScopeRepository>();
builder.Services.AddScoped<IPlanRouteRepository, PlanRouteRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IUsageLogRepository, UsageLogRepository>();
#endregion

#region 🔹 AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion

#region 🔹 JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new Exception("Missing Jwt SecretKey")))
        };
    });
#endregion

#region 🔹 Redis & Caching
var redisConnection = builder.Configuration.GetConnectionString("Redis")
                       ?? throw new Exception("Missing Redis connection string");

// Redis connection (singleton)
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisConnection));

// Distributed cache via Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = "AkamGateway:";
});

// Memory cache (local per instance)
builder.Services.AddMemoryCache();

// Hybrid cache (Memory + Redis)
builder.Services.AddScoped<IHybridCacheService, HybridCacheService>();

// Redis Pub/Sub Listener
builder.Services.AddHostedService<RedisSubscriberService>();
#endregion

#region 🔹 Usage Log Queue (Meta Data Log)
builder.Services.Configure<UsageLogSettings>(
    builder.Configuration.GetSection("UsageLogSettings"));

builder.Services.AddSingleton<IUsageLogQueueService, UsageLogQueueService>();
builder.Services.AddHostedService<UsageLogBackgroundService>();
#endregion

#region 🔹 Cache Warmup (Business cache)
builder.Services.AddScoped<ICacheLoader, CacheLoader>();
builder.Services.AddScoped<ICacheRefresher, CacheRefresher>();
builder.Services.AddHostedService<CacheWarmupHostedService>();
#endregion

#region 🔹 YARP Configuration
builder.Services.AddSingleton<DynamicYarpConfigProvider>();
builder.Services.AddSingleton<IProxyConfigProvider>(sp => sp.GetRequiredService<DynamicYarpConfigProvider>());
builder.Services.AddReverseProxy();
builder.Services.AddHostedService<YarpRoutesWarmupService>(); // Warmup Redis → reload YARP


#endregion

#region 🔹 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Gateway Public API", Version = "v1" });

    // JWT Bearer Support
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token below (without 'Bearer ')"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

builder.Services.AddControllers();

#region 🔹 Build app
var app = builder.Build();
#endregion

#region 🔹 Pipeline Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Gateway Middlewares (custom logic)
app.UseMiddleware<AccessAuthorizationMiddleware>();
app.UseMiddleware<PlanValidationMiddleware>();
app.UseMiddleware<ForwardPreviewLoggingMiddleware>();
app.UseMiddleware<UsageLoggingMiddleware>();
// YARP Reverse Proxy (MUST be last before MapControllers)
app.MapReverseProxy();

// Controllers (optional for admin/test APIs)
app.MapControllers();

app.Run();
#endregion
