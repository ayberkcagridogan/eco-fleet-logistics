using EcoFleetLogistics.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoFleetLogistics.Infrastructure.Persistence.Configurations;


public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TrackingNumber)
                    .IsRequired()
                    .HasMaxLength(50);
        
        builder.HasIndex(e => e.TrackingNumber)
                    .IsUnique();

        builder.Property(e => e.ReceiverName)
                    .IsRequired()
                    .HasMaxLength(50);
        
        builder.Property(e => e.SenderName)
                    .IsRequired()
                    .HasMaxLength(50);
        
        builder.Property(e => e.DestinationAddress)
                    .IsRequired()
                    .HasMaxLength(500);
        
        builder.Property(e => e.Weight)
                    .IsRequired();
        
        builder.Property(e => e.Status)
                    .HasConversion<string>()
                    .IsRequired();
        
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}