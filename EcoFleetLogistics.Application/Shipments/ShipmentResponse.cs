namespace EcoFleetLogistics.Application.Shipments;

public record ShipmentResponse(
    Guid Id,
    string TrackingNumber,
    string SenderName,
    string ReceiverName,
    string DestinationAddress,
    double Weight,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
    );