using EcoFleetLogistics.Application.Common.Persistence;
using MediatR;

namespace EcoFleetLogistics.Application.Shipments.Commands.DeleteShipment;

public record DeleteShipmentCommand(Guid Id) : IRequest<bool>;


public class DeleteShipmentCommandHandler : IRequestHandler<DeleteShipmentCommand, bool>
{
    private readonly IShipmentRepo _shipmentRepo;

    public DeleteShipmentCommandHandler(IShipmentRepo shipmentRepo)
    {
        _shipmentRepo = shipmentRepo;
    }
    public async Task<bool> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepo.GetByIdAsync(request.Id, cancellationToken);

        if(shipment == null)
            return false;

        shipment.Delete();
        await _shipmentRepo.UpdateAsync(shipment, cancellationToken);
        return true;
    }
}