using System.Collections.Concurrent;
using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Application.Common.Interfaces.Persistence;
using EcoFleetLogistics.Application.Common.Persistence;
using EcoFleetLogistics.Domain.Shipments;
using MediatR;

namespace EcoFleetLogistics.Application.Shipments.Commands.CreateShipment;

public record CreateShipmentCommand(
    string SenderName,
    string ReceiverName,
    string DestinationAddress,
    double Weight,
    Guid? DriverId) : IRequest<Guid>;

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Guid>
{
    private readonly IShipmentRepo _shipmentRepo;
    private readonly IUnityOfWork _unityOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateShipmentCommandHandler(IShipmentRepo shipmentRepo, IUnityOfWork unityOfWork, ICurrentUserService currentUserService)
    {
        _shipmentRepo = shipmentRepo;
        _unityOfWork = unityOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUserService.CompanyId 
            ?? throw new UnauthorizedAccessException("Tenant/Company context is missing.");

        var createdById = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User context is missing.");

        var shipment = Shipment.Create(            
            senderName: request.SenderName,
            receiverName: request.ReceiverName,
            destinationAddress: request.DestinationAddress,
            weight: request.Weight,
            companyId : companyId,
            createdById : createdById,
            driverId : request.DriverId
        );

        await _shipmentRepo.AddAsync(shipment, cancellationToken);
        await _unityOfWork.SaveChangesAsync(cancellationToken);
        return shipment.Id;
    }
} 