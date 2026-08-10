using EcoFleet.Shipment.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoFleet.Shipment.Infrastructure.Persistence.Configurations;


public class ShipmentConfiguration : IEntityTypeConfiguration<Domain.Shipments.Shipment>
{
    public void Configure(EntityTypeBuilder<Domain.Shipments.Shipment> builder)
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
        
     /*   builder.HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.CreatedBy)
            .WithMany()
            .HasForeignKey(s => s.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(s => s.Driver)
            .WithMany()
            .HasForeignKey(s => s.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
            */
    }
}