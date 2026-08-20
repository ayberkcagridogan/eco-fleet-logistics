
using System.Linq.Expressions;
using EcoFleet.Shared.Kernel.Persistence;
using EcoFleet.Shipment.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcoFleet.Shipment.Infrastructure.Persistence.Repositories
{
    public class ShipmentRepo : RepositoryBase<Domain.Shipments.Shipment, Guid, ShipmentDbContext>, IShipmentRepo
    {

        public ShipmentRepo(ShipmentDbContext context)
        : base(context) {}

        protected ShipmentRepo(
            ShipmentDbContext context, 
            bool isTenantFilterIgnored,
            bool isNoTrackingEnabled,
            List<Expression<Func<Domain.Shipments.Shipment, object>>> includes)
        :base(context, isTenantFilterIgnored, isNoTrackingEnabled, includes ){}

        public new IShipmentRepo IgnoreTenantFilter() => (IShipmentRepo)base.IgnoreTenantFilter();
        public new IShipmentRepo AsNoTracking() => (IShipmentRepo)base.AsNoTracking();
        public new IShipmentRepo Include(Expression<Func<Domain.Shipments.Shipment, object>> navigationPropertyPath) 
                => (IShipmentRepo)base.Include(navigationPropertyPath);
/*
        public async Task<Domain.Shipments.Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber, cancellationToken);
        }


        public async Task<bool> ExistsByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                                    .AsNoTracking()
                                    .AnyAsync(s => s.TrackingNumber == trackingNumber, cancellationToken);
        }
*/
    }
}