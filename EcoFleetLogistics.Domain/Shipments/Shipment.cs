using EcoFleetLogistics.Domain.Shipments.Enums;

namespace EcoFleetLogistics.Domain.Shipments
{
    public class Shipment
    {
        public Guid Id { get; private set; }
        public string TrackingNumber { get; private set; }
        public string SenderName { get; private set; }
        public string ReceiverName { get; private set; }
        public string DestinationAddress { get; private set; }
        public double Weight { get; private set; }
        public ShipmentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Shipment() { } // Parameterless constructor for EF Core

        private Shipment(Guid id, string trackingNumber, string senderName, string receiverName, string destinationAddress, double weight)
        {
            Id = id;
            TrackingNumber = trackingNumber;
            SenderName = senderName;
            ReceiverName = receiverName;
            DestinationAddress = destinationAddress;
            Weight = weight;
            Status = ShipmentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }
        public static Shipment Create(string trackingNumber, string senderName, string receiverName, string destinationAddress, double weight)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty.");

            if (string.IsNullOrWhiteSpace(senderName))
                throw new ArgumentException("Sender name cannot be empty.");

            if (string.IsNullOrWhiteSpace(receiverName))
                throw new ArgumentException("Receiver name cannot be empty.");

            if (string.IsNullOrWhiteSpace(destinationAddress))
                throw new ArgumentException("Destination cannot be empty.");

            if (weight <= 0)
                throw new ArgumentException("Weight must be greater than zero.");

            return new Shipment(Guid.NewGuid(), trackingNumber, senderName, receiverName, destinationAddress, weight);
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
        public void ChangeStatus(ShipmentStatus newStatus)
        {
            if (Status == newStatus)
                return;


            if(Status == ShipmentStatus.Delivered)
            {
                throw new InvalidOperationException("A 'Delivered' shipment cannot be modified.");
            }
            if (Status == ShipmentStatus.Cancelled)
            {
                throw new InvalidOperationException("A 'Cancelled' shipment cannot be modified.");
            }
            if (Status == ShipmentStatus.Pending && newStatus == ShipmentStatus.Delivered)
            {
                throw new InvalidOperationException("A shipment cannot transition directly from 'Pending' to 'Delivered'. It must be 'InTransit' first.");
            }
            if (Status == ShipmentStatus.InTransit && newStatus == ShipmentStatus.Cancelled)
            {
                throw new InvalidOperationException("An 'InTransit' shipment cannot be cancelled.");
            }

            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
