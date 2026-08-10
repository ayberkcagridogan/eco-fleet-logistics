using EcoFleet.Shared.Kernel.Persistence;
using EcoFleet.Shared.Kernel.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoFleet.Shipment.Infrastructure.Persistence;

public class ShipmentDbContext : BaseDbContext<ShipmentDbContext>
{
    public DbSet<Domain.Shipments.Shipment> Shipments => Set<Domain.Shipments.Shipment>();
    public ShipmentDbContext(
        DbContextOptions<ShipmentDbContext> options,
        ICurrentUserService currentUserService) : base(options, currentUserService)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Shipment");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShipmentDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}