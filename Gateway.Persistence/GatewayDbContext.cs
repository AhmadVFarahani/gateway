using Gateway.Domain.Entities;
using Gateway.Domain.Views;
using Gateway.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence;
//dotnet ef migrations add  --startup-project Gateway.Admin.API --project Gateway.Persistence
//dotnet ef database update --startup-project Gateway.Admin.API --project Gateway.Persistence

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
    public DbSet<RouteRequestField> RouteRequestFields { get; set; }
    public DbSet<RouteResponseField> RouteResponseFields { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Role> Roles { get; set; }

    public DbSet<Plan> Plans { get; set; }
    public DbSet<PlanRoute> PlanRoutes { get; set; }
    public DbSet<Contract> Contracts { get; set; }

    public DbSet<UsageLog> UsageLogs { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }


    public DbSet<InvoiceView> InvoiceViews { get; set; }
    public DbSet<InvoiceItemView> InvoiceItemViews { get; set; }

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
        modelBuilder.ApplyConfiguration(new RouteRequestFieldConfiguration());
        modelBuilder.ApplyConfiguration(new RouteResponseFieldConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());

        modelBuilder.ApplyConfiguration(new PlanConfiguration());
        modelBuilder.ApplyConfiguration(new PlanRouteConfiguration());
        modelBuilder.ApplyConfiguration(new ContractConfiguration());

        modelBuilder.ApplyConfiguration(new UsageLogConfig());
        modelBuilder.ApplyConfiguration(new InvoiceConfig());
        modelBuilder.ApplyConfiguration(new InvoiceItemConfig());


        modelBuilder.Entity<InvoiceView>()
                .HasNoKey()
                .ToView("v_Invoice_List");
        modelBuilder.Entity<InvoiceItemView>()
               .HasNoKey()
               .ToView("v_Invoice_Item");
    }
}
