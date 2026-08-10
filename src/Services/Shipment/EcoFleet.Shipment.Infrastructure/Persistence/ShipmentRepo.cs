
using EcoFleet.Shipment.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcoFleet.Shipment.Infrastructure.Persistence.Repositories
{
        public class ShipmentRepo : IShipmentRepo
    {
        private readonly ShipmentDbContext _context;

        public ShipmentRepo(ShipmentDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Shipments.Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<Domain.Shipments.Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber, cancellationToken);
        }

        public async Task AddAsync(Domain.Shipments.Shipment shipment, CancellationToken cancellationToken = default)
        {
            await _context.Shipments.AddAsync(shipment, cancellationToken);
        }

        public void Update(Domain.Shipments.Shipment shipment , CancellationToken cancellationToken = default)
        {
            _context.Shipments.Update(shipment);
        }

        public async Task<bool> ExistsByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                                    .AsNoTracking()
                                    .AnyAsync(s => s.TrackingNumber == trackingNumber, cancellationToken);
        }

        public void Remove(Domain.Shipments.Shipment shipment)
        {
            _context.Shipments.Remove(shipment);
        }
    }
}