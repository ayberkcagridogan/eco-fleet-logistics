using FluentValidation;

namespace EcoFleetLogistics.Application.Shipments.Commands.UpdateShipment;

public class UpdateShipmentCommandValidator : AbstractValidator<UpdateShipmentCommand>
{
    public UpdateShipmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Shipment ID is required.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.ReceiverName) || !string.IsNullOrWhiteSpace(x.DestinationAddress))
            .WithMessage("At least one field (ReceiverName or DestinationAddress) must be provided for update.");

        When(x =>!string.IsNullOrWhiteSpace(x.ReceiverName),() =>
        {
            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage("Receiver name cannot be empty when provided.")
                .MinimumLength(2).WithMessage("Receiver name must be at least 5 characters long.")
                .MaximumLength(50).WithMessage("Receiver name must not exceed 50 characters.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.DestinationAddress),() =>
        {
            RuleFor(x => x.DestinationAddress)
                .NotEmpty().WithMessage("Destination address cannot be empty when provided.")
                .MinimumLength(10).WithMessage("Destination address must be at least 10 characters long.")
                .MaximumLength(500).WithMessage("Destination address cannot exceed 500 characters.");
        });
    }
}