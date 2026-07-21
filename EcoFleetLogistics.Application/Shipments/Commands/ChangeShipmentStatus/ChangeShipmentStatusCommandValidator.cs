using FluentValidation;

namespace EcoFleetLogistics.Application.Shipments.Commands.ChangeShipmentStatus;

public class ChangeShipmentStatusCommandValidator : AbstractValidator<ChangeShipmentStatusCommand>
{
    public ChangeShipmentStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Shipment ID is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid shipment status.");
    }
}