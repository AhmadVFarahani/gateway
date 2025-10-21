using Gateway.Application.Implementations;
using Gateway.Application.Implementations.Cache;
using Gateway.Application.Interfaces;
using Gateway.Application.Interfaces.Cache;
using Gateway.Domain.Interfaces;
using Gateway.Infrastructure.Repositories;
using Gateway.Persistence;
using Gateway.Public.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<GatewayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// JWT Authentication Setup
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
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new Exception("Missing secret"))),
        };
    });


// Redis Connection Multiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(configuration);
});

// Distributed Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "AkamGateway:";
});

// Memory cache
builder.Services.AddMemoryCache();

//Cache Loader
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

builder.Services.AddScoped<ICacheLoader, CacheLoader>();
builder.Services.AddScoped<ICacheRefresher, CacheRefresher>();
builder.Services.AddHostedService<CacheWarmupHostedService>();



// Redis connection & distributed cache
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var cs = builder.Configuration.GetConnectionString("Redis"); 
    return ConnectionMultiplexer.Connect(cs);
});
builder.Services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = builder.Configuration.GetConnectionString("Redis");
    o.InstanceName = builder.Configuration.GetSection("Cache")?["InstanceName"] ?? "AkamGateway:";
});

// Hybrid cache
builder.Services.AddScoped<IHybridCacheService, HybridCacheService>();

// Redis Pub/Sub Subscriber
builder.Services.AddHostedService<RedisSubscriberService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Gateway Public API", Version = "v1" });

    // Add JWT Bearer definition
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token below (without 'Bearer ')"
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


builder.Services.AddScoped<ICacheService, RedisCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<ScopeAuthorizationMiddleware>();

app.MapControllers();

app.Run();
