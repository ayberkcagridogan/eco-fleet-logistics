using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
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
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShipmentCommandHandler(IShipmentRepo shipmentRepo, IUnitOfWork unitOfWork)
    {
        _shipmentRepo = shipmentRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepo.GetByIdAsync(request.Id, cancellationToken);

        if (shipment == null)
            return false;

        shipment.UpdateDetails(request.ReceiverName, request.DestinationAddress);
         _shipmentRepo.Update(shipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}