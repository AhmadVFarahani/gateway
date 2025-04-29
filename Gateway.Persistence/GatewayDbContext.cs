using Gateway.Domain.Entities;
using Gateway.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence;

public class GatewayDbContext : DbContext
{
    public GatewayDbContext(DbContextOptions<GatewayDbContext> options)
    : base(options)
    {
    }

    public DbSet<Service> Services { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<Scope> Scopes { get; set; }
    public DbSet<AccessPolicy> AccessPolicies { get; set; }
    public DbSet<RouteScope> RouteScopes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ServiceConfiguration());
        modelBuilder.ApplyConfiguration(new RouteConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ApiKeyConfiguration());
        modelBuilder.ApplyConfiguration(new ScopeConfiguration());
        modelBuilder.ApplyConfiguration(new AccessPolicyConfiguration());
        modelBuilder.ApplyConfiguration(new RouteConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
}
