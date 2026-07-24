using EcoFleetLogistics.Domain.Shipments;

namespace EcoFleetLogistics.Application.Common.Persistence
{
    public interface IShipmentRepo
    {
        Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
        Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default);
        Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
    }
}