using FluentValidation;

namespace EcoFleetLogistics.Application.Shipments.Commands.CreateShipment;

public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.TrackingNumber)
            .NotEmpty().WithMessage("Tracking number is required.")
            .MinimumLength(5).WithMessage("Tracking number must be at least 5 characters long.")
            .MaximumLength(50).WithMessage("Tracking number cannot exceed 50 characters.");

        RuleFor(x => x.SenderName)
            .NotEmpty().WithMessage("Sender name is required.")
            .MaximumLength(50).WithMessage("Sender name cannot exceed 50 characters.");
        
        RuleFor(x => x.ReceiverName)
            .NotEmpty().WithMessage("Receiver name is required.")
            .MaximumLength(50).WithMessage("Receiver name cannot exceed 50 characters.");
        
        RuleFor(x => x.DestinationAddress)
            .NotEmpty().WithMessage("Destination address is required.")
            .MinimumLength(10).WithMessage("Destination address must be at least 10 characters long.")
            .MaximumLength(500).WithMessage("Destination address cannot exceed 500 characters.");
        
        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0 kg.");
    }
}