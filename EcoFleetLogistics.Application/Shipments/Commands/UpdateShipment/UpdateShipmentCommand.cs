using EcoFleetLogistics.Application.Common.Persistence;
using MediatR;

namespace EcoFleetLogistics.Application.Shipments.Commands.UpdateShipment;

public record UpdateShipmentCommand(
    Guid Id,
    string? ReceiverName = null,
    string? DestinationAddress = null) : IRequest<bool>;


public class UpdateShipmentCommandHandler : IRequestHandler<UpdateShipmentCommand, bool>
{
    private readonly IShipmentRepo _shipmentRepo;

    public UpdateShipmentCommandHandler(IShipmentRepo shipmentRepo)
    {
        _shipmentRepo = shipmentRepo;
    }

    public async Task<bool> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepo.GetByIdAsync(request.Id, cancellationToken);

        if (shipment == null)
            return false;

        shipment.UpdateDetails(request.ReceiverName, request.DestinationAddress);
        await _shipmentRepo.UpdateAsync(shipment, cancellationToken);

        return true;
    }
}