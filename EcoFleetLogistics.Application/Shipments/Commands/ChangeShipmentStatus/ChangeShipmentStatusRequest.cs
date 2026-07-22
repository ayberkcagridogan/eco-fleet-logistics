using EcoFleetLogistics.Domain.Shipments.Enums;

namespace EcoFleetLogistics.Application.Shipments.Commands.ChangeShipmentStatus;

public record ChangeShipmentStatusRequest(ShipmentStatus NewStatus);