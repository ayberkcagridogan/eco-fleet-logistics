namespace EcoFleetLogistics.Infrastructure.Persistence.Repositories
{
    using EcoFleetLogistics.Application.Common.Interfaces;
    using EcoFleetLogistics.Domain.Shipments;
    using Microsoft.EntityFrameworkCore;

    public class ShipmentRepo : IShipmentRepo
    {
        private readonly AppDbContext _context;

        public ShipmentRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments.FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber, cancellationToken);
        }

        public async Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default)
        {
            await _context.Shipments.AddAsync(shipment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Shipment shipment , CancellationToken cancellationToken = default)
        {
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}