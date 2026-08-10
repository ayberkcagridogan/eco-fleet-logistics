namespace EcoFleet.Shipment.Application.Shipments.Commands.UpdateShipment;

public record UpdateShipmentRequest(
    string? ReceiverName = null,
    string? DestinationAddress = null);