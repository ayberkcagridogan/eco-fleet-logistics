using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
using MediatR;

namespace EcoFleetLogistics.Application.Shipments.Commands.DeleteShipment;

public record DeleteShipmentCommand(Guid Id) : IRequest<bool>;


public class DeleteShipmentCommandHandler : IRequestHandler<DeleteShipmentCommand, bool>
{
    private readonly IShipmentRepo _shipmentRepo;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteShipmentCommandHandler(IShipmentRepo shipmentRepo, IUnitOfWork unitOfWork)
    {
        _shipmentRepo = shipmentRepo;
        _unitOfWork = unitOfWork;
    }
    public async Task<bool> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepo.GetByIdAsync(request.Id, cancellationToken);

        if(shipment == null)
            return false;

        shipment.Delete();
        _shipmentRepo.Update(shipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}