using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Domain.Shipments;
using MediatR;

namespace EcoFleetLogistics.Application.Shipments.Commands.CreateShipment;

public record CreateShipmentCommand(
    string TrackingNumber,
    string SenderName,
    string ReceiverName,
    string DestinationAddress,
    double Weight) : IRequest<Guid>;

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Guid>
{
    private readonly IShipmentRepo _shipmentRepo;

    public CreateShipmentCommandHandler(IShipmentRepo shipmentRepo)
    {
        _shipmentRepo = shipmentRepo;
    }

    public async Task<Guid> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        var shipment = Shipment.Create(
            trackingNumber: request.TrackingNumber,
            senderName: request.SenderName,
            receiverName: request.ReceiverName,
            destinationAddress: request.DestinationAddress,
            weight: request.Weight
        );

        await _shipmentRepo.AddAsync(shipment, cancellationToken);
        return shipment.Id;
    }
} 