using FluentValidation;

namespace EcoFleet.Shipment.Application.Shipments.Commands.CreateShipment;

public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
 //  private readonly IUserRepo _userRepo;

    public CreateShipmentCommandValidator()
    {
    //    _userRepo = userRepo;

        RuleFor(x => x.SenderName)
            .NotEmpty().WithMessage("Sender name is required.")
            .MinimumLength(2).WithMessage("Sender name must be at least 5 characters long.")
            .MaximumLength(50).WithMessage("Sender name cannot exceed 50 characters.");
        
        RuleFor(x => x.ReceiverName)
            .NotEmpty().WithMessage("Receiver name is required.")
            .MinimumLength(2).WithMessage("Receiver name must be at least 5 characters long.")
            .MaximumLength(50).WithMessage("Receiver name cannot exceed 50 characters.");
        
        RuleFor(x => x.DestinationAddress)
            .NotEmpty().WithMessage("Destination address is required.")
            .MinimumLength(10).WithMessage("Destination address must be at least 10 characters long.")
            .MaximumLength(500).WithMessage("Destination address cannot exceed 500 characters.");
        
        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0 kg.");

        RuleFor(x => x.DriverId)
            .MustAsync(BeValidDriverAsync)
            .When(x => x.DriverId.HasValue)
            .WithMessage("The assigned user was not found or does not have the 'Driver' role.");
    }

    private async Task<bool> BeValidDriverAsync(Guid? driverId, CancellationToken cancellationToken)
    {
        /*
        if(!driverId.HasValue)
            return true;
        var driver = await _userRepo.GetByIdAsync(driverId.Value,cancellationToken);

        return driver != null && driver.Role == UserRole.Driver;
        */

        return true; // Placeholder for actual driver validation logic
    }
}