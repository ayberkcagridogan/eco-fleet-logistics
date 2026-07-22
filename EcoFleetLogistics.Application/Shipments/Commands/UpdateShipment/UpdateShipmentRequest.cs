namespace EcoFleetLogistics.Application.Shipments.Commands.UpdateShipment;

public record UpdateShipmentRequest(
    string? ReceiverName = null,
    string? DestinationAddress = null);