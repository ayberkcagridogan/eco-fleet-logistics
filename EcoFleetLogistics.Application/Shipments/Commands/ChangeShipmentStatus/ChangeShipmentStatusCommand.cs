
using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.Shipments.Enums;
using MediatR;

namespace EcoFleetLogistics.Application.Shipments.Commands.ChangeShipmentStatus;

public record ChangeShipmentStatusCommand(Guid Id, ShipmentStatus NewStatus) : IRequest<bool>;

public class ChangeShipmentStatusCommandHandler : IRequestHandler<ChangeShipmentStatusCommand, bool>
{
    private readonly IShipmentRepo _shipmentRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeShipmentStatusCommandHandler(IShipmentRepo shipmentRepo, IUnitOfWork unitOfWork)
    {
        _shipmentRepo = shipmentRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ChangeShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepo.GetByIdAsync(request.Id, cancellationToken);
        if(shipment == null)
            return false;
        
        switch (request.NewStatus)
        {
            case ShipmentStatus.InTransit:
                shipment.StartTransit();
                break;
            case ShipmentStatus.Delivered:
                shipment.MarkAsDelivered();
                break;
            case ShipmentStatus.Cancelled:
                shipment.Cancel();
                break;
            default:
                throw new InvalidOperationException($"Unsupported status transition to {request.NewStatus}.");
        }
        _shipmentRepo.Update(shipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

