using EcoFleet.Shipment.Application.Common.Persistence;
using MediatR;

namespace EcoFleet.Shipment.Application.Shipments.Commands.CreateShipment;

public record CreateShipmentCommand(
    string SenderName,
    string ReceiverName,
    string DestinationAddress,
    double Weight,
    Guid? DriverId) : IRequest<Guid>;

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Guid>
{
    private readonly IShipmentRepo _shipmentRepo;
  //  private readonly IUnityOfWork _unityOfWork;
   // private readonly ICurrentUserService _currentUserService;

    public CreateShipmentCommandHandler(IShipmentRepo shipmentRepo
    //, IUnityOfWork unityOfWork, ICurrentUserService currentUserService
    )
    {
        _shipmentRepo = shipmentRepo;
      //  _unityOfWork = unityOfWork;
        //_currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        /**
        var companyId = _currentUserService.CompanyId 
            ?? throw new UnauthorizedAccessException("Tenant/Company context is missing.");

        var createdById = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User context is missing.");
            */

        var shipment = Domain.Shipments.Shipment.Create(            
            senderName: request.SenderName,
            receiverName: request.ReceiverName,
            destinationAddress: request.DestinationAddress,
            weight: request.Weight,
            tenantId : Guid.NewGuid(), // Replace with actual tenantId from context
            createdById : Guid.NewGuid(), // Replace with actual createdById from context
            driverId : request.DriverId
        );

        await _shipmentRepo.AddAsync(shipment, cancellationToken);
    //    await _unityOfWork.SaveChangesAsync(cancellationToken);
        return shipment.Id;
    }
} 