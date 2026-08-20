
using System.Linq.Expressions;
using EcoFleet.Shared.Kernel.Persistence.Interfaces;

namespace EcoFleet.Shipment.Application.Common.Persistence
{
    public interface IShipmentRepo : IRepositoryBase<Domain.Shipments.Shipment, Guid>
    {
        new IShipmentRepo IgnoreTenantFilter();
        new IShipmentRepo AsNoTracking();
        new IShipmentRepo Include(Expression<Func<Domain.Shipments.Shipment, object>> navigationPropertyPath);
    }
}