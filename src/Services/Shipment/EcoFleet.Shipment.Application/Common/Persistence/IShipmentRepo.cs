
namespace EcoFleet.Shipment.Application.Common.Persistence
{
    public interface IShipmentRepo
    {
        Task<Domain.Shipments.Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Domain.Shipments.Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Shipments.Shipment shipment, CancellationToken cancellationToken = default);
        void Update(Domain.Shipments.Shipment shipment, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
        void Remove(Domain.Shipments.Shipment shipment);
    }
}