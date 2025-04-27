using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Persistence;

public class GatewayDbContext : DbContext
{
    public GatewayDbContext(DbContextOptions<GatewayDbContext> options)
    : base(options)
    {
    }

    public DbSet<Service> Services { get; set; }
}
