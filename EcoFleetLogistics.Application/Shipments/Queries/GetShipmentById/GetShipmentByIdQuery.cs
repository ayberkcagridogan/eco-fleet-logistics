using EcoFleetLogistics.Application.Common.Interfaces;
using MediatR;

namespace EcoFleetLogistics.Application.Shipments.Queries.GetShipmentById;

public record GetShipmentByIdQuery(Guid Id) : IRequest<ShipmentResponse>;

public class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, ShipmentResponse?>
{
    private readonly IShipmentRepo _shipmentRepo;
    public GetShipmentByIdQueryHandler(IShipmentRepo shipmentRepo)
    {
        _shipmentRepo = shipmentRepo;
    }
    public async Task<ShipmentResponse?> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepo.GetByIdAsync(request.Id, cancellationToken);
        if (shipment is null) return null;
        return new ShipmentResponse(
            shipment.Id,
            shipment.TrackingNumber,
            shipment.SenderName,
            shipment.ReceiverName,
            shipment.DestinationAddress,
            shipment.Weight,
            shipment.Status.ToString(),
            shipment.CreatedAt,
            shipment.UpdatedAt
        );
    }
}