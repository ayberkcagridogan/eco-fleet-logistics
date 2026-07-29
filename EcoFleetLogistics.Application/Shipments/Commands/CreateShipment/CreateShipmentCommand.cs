using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
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
    private readonly IUnityOfWork _unityOfWork;

    public CreateShipmentCommandHandler(IShipmentRepo shipmentRepo, IUnityOfWork unityOfWork)
    {
        _shipmentRepo = shipmentRepo;
        _unityOfWork = unityOfWork;
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
        await _unityOfWork.SaveChangesAsync(cancellationToken);
        return shipment.Id;
    }
} 