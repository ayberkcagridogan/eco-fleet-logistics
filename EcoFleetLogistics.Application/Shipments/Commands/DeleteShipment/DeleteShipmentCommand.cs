using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
using MediatR;

namespace EcoFleetLogistics.Application.Shipments.Commands.DeleteShipment;

public record DeleteShipmentCommand(Guid Id) : IRequest<bool>;


public class DeleteShipmentCommandHandler : IRequestHandler<DeleteShipmentCommand, bool>
{
    private readonly IShipmentRepo _shipmentRepo;
    private readonly IUnityOfWork _unityOfWork;

    public DeleteShipmentCommandHandler(IShipmentRepo shipmentRepo, IUnityOfWork unityOfWork)
    {
        _shipmentRepo = shipmentRepo;
        _unityOfWork = unityOfWork;
    }
    public async Task<bool> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepo.GetByIdAsync(request.Id, cancellationToken);

        if(shipment == null)
            return false;

        shipment.Delete();
        _shipmentRepo.Update(shipment, cancellationToken);
        await _unityOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}