using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Domain.Shipments.Enums;
using MediatR;

namespace EcoFleetLogistics.Application.Shipments.Commands.ChangeShipmentStatus;

public record ChangeShipmentStatusCommand(Guid Id, ShipmentStatus NewStatus) : IRequest<bool>;

public class ChangeShipmentStatusCommandHandler : IRequestHandler<ChangeShipmentStatusCommand, bool>
{
    private readonly IShipmentRepo _shipmentRepo;

    public ChangeShipmentStatusCommandHandler(IShipmentRepo shipmentRepo)
    {
        _shipmentRepo = shipmentRepo;
    }

    public async Task<bool> Handle(ChangeShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepo.GetByIdAsync(request.Id, cancellationToken);
        if(shipment == null)
            return false;
        
        shipment.ChangeStatus(request.NewStatus);
        await _shipmentRepo.UpdateAsync(shipment, cancellationToken);
        return true;
    }
}

