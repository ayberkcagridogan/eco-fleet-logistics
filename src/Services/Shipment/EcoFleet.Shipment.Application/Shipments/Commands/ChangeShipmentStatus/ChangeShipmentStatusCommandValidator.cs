using EcoFleet.Shipment.Domain.Shipments.Enums;
using FluentValidation;

namespace EcoFleet.Shipment.Application.Shipments.Commands.ChangeShipmentStatus;

public class ChangeShipmentStatusCommandValidator : AbstractValidator<ChangeShipmentStatusCommand>
{
 //   private readonly IUserRepo _userRepo;

    public ChangeShipmentStatusCommandValidator()
    {
     //   _userRepo = userRepo;
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Shipment ID is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid shipment status.");

        RuleFor(x => x.DriverId)
            .NotEmpty()
            .WithMessage("A valid DriverId must be specified when setting the shipment status to 'Assigned'.")
            .When(x => x.NewStatus == ShipmentStatus.Assigned);
        
        RuleFor(x => x.DriverId)
            .MustAsync(BeValidDriverAsync)
            .When(x => x.DriverId.HasValue)
            .WithMessage("The assigned user was not found or does not have the 'Driver' role.");
    }

    private async Task<bool> BeValidDriverAsync(Guid? driverId, CancellationToken cancellationToken)
    {
     /*   if(!driverId.HasValue)
            return true;
        var driver = await _userRepo.GetByIdAsync(driverId.Value,cancellationToken);

        return driver != null && driver.Role == UserRole.Driver;*/
        return true;
    }
}