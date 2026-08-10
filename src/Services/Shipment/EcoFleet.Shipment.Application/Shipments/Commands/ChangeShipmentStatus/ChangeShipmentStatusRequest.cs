using EcoFleet.Shipment.Domain.Shipments.Enums;

namespace EcoFleet.Shipment.Application.Shipments.Commands.ChangeShipmentStatus;

public record ChangeShipmentStatusRequest(ShipmentStatus NewStatus, Guid? DriverId = null);