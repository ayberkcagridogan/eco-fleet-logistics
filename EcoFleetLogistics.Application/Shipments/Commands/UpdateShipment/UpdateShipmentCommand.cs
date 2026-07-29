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
    private readonly IUnityOfWork _unityOfWork;

    public UpdateShipmentCommandHandler(IShipmentRepo shipmentRepo, IUnityOfWork unityOfWork)
    {
        _shipmentRepo = shipmentRepo;
        _unityOfWork = unityOfWork;
    }

    public async Task<bool> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepo.GetByIdAsync(request.Id, cancellationToken);

        if (shipment == null)
            return false;

        shipment.UpdateDetails(request.ReceiverName, request.DestinationAddress);
         _shipmentRepo.Update(shipment, cancellationToken);
        await _unityOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}