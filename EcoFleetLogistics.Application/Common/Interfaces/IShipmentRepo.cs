using EcoFleetLogistics.Domain.Shipments;

namespace EcoFleetLogistics.Application.Common.Interfaces
{
    public interface IShipmentRepo
    {
        Task<Shipment?> GetByIdAsync(Guid id);
        Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber);
        Task AddAsync(Shipment shipment);
        Task UpdateAsync(Shipment shipment);
    }
}