
using EcoFleet.Shared.Kernel.Primitives;
using EcoFleet.Shared.Kernel.Primitives.Interfaces;
using EcoFleet.Shipment.Domain.Shipments.Enums;

namespace EcoFleet.Shipment.Domain.Shipments
{
    public class Shipment : BaseEntity<Guid> , IMultiTenant
    {
        public string TrackingNumber { get; private set; } = null!;
        public string SenderName { get; private set; } = null!;
        public string ReceiverName { get; private set; } = null!;
        public string DestinationAddress { get; private set; } = null!;
        public double Weight { get; private set; }
        public ShipmentStatus Status { get; private set; }
  //      public Company Company { get; private set; } = null!;
  //      public User CreatedBy { get; private set; } = null!;
        public Guid? DriverId { get; private set; }

        public Guid TenantId { get; private set; }

        //    public User? Driver { get; private set; }

        // Parameterless constructor for EF Core
        private Shipment() {}     
        private Shipment(
            Guid id,
            string trackingNumber,
            string senderName, 
            string receiverName, 
            string destinationAddress,
            double weight,
            ShipmentStatus status,
            Guid tenantId,
            Guid createdById,
            Guid? driverId = null,
            DateTime? updatedAt = null)
        {
            Id = id;
            TrackingNumber = trackingNumber;
            SenderName = senderName;
            ReceiverName = receiverName;
            DestinationAddress = destinationAddress;
            Weight = weight;
            Status = status;
            TenantId = tenantId;
            CreatedById = createdById;
            DriverId = driverId;
            UpdatedAt = updatedAt;
        }
        public static Shipment Create( 
            string senderName,
            string receiverName, 
            string destinationAddress, 
            double weight,
            Guid tenantId,
            Guid createdById,
            Guid? driverId = null)
        {
            if (string.IsNullOrWhiteSpace(senderName))
                throw new ArgumentException("Sender name cannot be empty.", nameof(senderName));

            if (string.IsNullOrWhiteSpace(receiverName))
                throw new ArgumentException("Receiver name cannot be empty.", nameof(receiverName));

            if (string.IsNullOrWhiteSpace(destinationAddress))
                throw new ArgumentException("Destination cannot be empty.", nameof(destinationAddress));

            if (weight <= 0)
                throw new ArgumentException("Weight must be greater than zero.", nameof(weight));

            return new Shipment(
                Guid.NewGuid(),
                $"TRK-{Guid.NewGuid().ToString()[..8].ToUpper()}", 
                senderName, 
                receiverName, 
                destinationAddress, 
                weight,
                driverId.HasValue ? ShipmentStatus.Assigned : ShipmentStatus.Pending,
                tenantId,
                createdById,
                driverId);
        }

        public void UpdateDetails(string? receiverName = null, string? destinationAddress = null)
        {
            EnsureNotFinalState();

            if (string.IsNullOrWhiteSpace(receiverName) && string.IsNullOrWhiteSpace(destinationAddress))
                throw new InvalidOperationException("At least one field (ReceiverName or DestinationAddress) must be provided for update.");

            if(Status == ShipmentStatus.InTransit)
                throw new InvalidOperationException("Cannot update details while shipment is In Transit.");

            if (!string.IsNullOrWhiteSpace(receiverName))
                ReceiverName = receiverName;
                
            if (!string.IsNullOrWhiteSpace(destinationAddress))
                DestinationAddress = destinationAddress;
        }

        public void AssignDriver(Guid driverId)
        {
            EnsureNotFinalState();

            if (Status == ShipmentStatus.InTransit)
                throw new InvalidOperationException("Cannot re-assign driver while shipment is In Transit.");

            DriverId = driverId;
            Status = ShipmentStatus.Assigned;   
        }
        public void StartTransit()
        {
            EnsureNotFinalState();

            if (!DriverId.HasValue)
                throw new InvalidOperationException("Cannot start transit without assigning a driver.");

            if (Status != ShipmentStatus.Assigned && Status != ShipmentStatus.Pending)
                throw new InvalidOperationException($"Cannot start transit. Current status is {Status}.");
          
            Status = ShipmentStatus.InTransit;
        }
        public void CompleteDelivery()
        {
            if (Status != ShipmentStatus.InTransit)
                throw new InvalidOperationException("A shipment can only be marked as delivered if it is currently In Transit.");

            Status = ShipmentStatus.Delivered;
        }
        public void Cancel()
        {
            if (Status == ShipmentStatus.InTransit)
                throw new InvalidOperationException("An 'InTransit' shipment cannot be cancelled.");

            if (Status == ShipmentStatus.Delivered)
                throw new InvalidOperationException("A 'Delivered' shipment cannot be cancelled.");

            Status = ShipmentStatus.Cancelled;
        }

        public void Delete()
        {
            if (Status == ShipmentStatus.InTransit)
                throw new InvalidOperationException("An 'InTransit' shipment cannot be deleted.");

            if (Status == ShipmentStatus.Delivered)
                throw new InvalidOperationException("A 'Delivered' shipment cannot be deleted.");
                
        }

        private void EnsureNotFinalState()
        {
            if (Status == ShipmentStatus.Delivered)
                throw new InvalidOperationException("A 'Delivered' shipment cannot be modified.");

            if (Status == ShipmentStatus.Cancelled)
                throw new InvalidOperationException("A 'Cancelled' shipment cannot be modified.");
        }
    }
}
