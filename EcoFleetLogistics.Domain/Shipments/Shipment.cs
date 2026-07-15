using EcoFleetLogistics.Domain.Shipments.Enums;

namespace EcoFleetLogistics.Domain.Shipments
{
    public class Shipment
    {
        public Guid Id { get; private set; }
        public string TrackingNumber { get; private set; }
        public string Destination { get; private set; }
        public double Weight { get; private set; }
        public ShipmentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Shipment(string trackingNumber, string destination, double weight)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.");

            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentException("Destination cannot be empty.");

            if (weight <= 0)
                throw new ArgumentException("Weight must be greater than zero.");

            Id = Guid.NewGuid();
            TrackingNumber = trackingNumber;
            Destination = destination;
            Weight = weight;
            Status = ShipmentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }
        public void StartTransit()
        {
            if (Status != ShipmentStatus.Pending)
            {
                throw new InvalidOperationException($"Cannot start transit. Current status is {Status}.");
            }

            Status = ShipmentStatus.InTransit;
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsDelivered()
        {
            if (Status != ShipmentStatus.InTransit)
            {
                throw new InvalidOperationException("A shipment can only be marked as delivered if it is currently In Transit.");
            }

            Status = ShipmentStatus.Delivered;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Cancel()
        {
            if (Status != ShipmentStatus.Pending)
            {
                throw new InvalidOperationException("Only pending shipments can be cancelled.");
            }

            Status = ShipmentStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
