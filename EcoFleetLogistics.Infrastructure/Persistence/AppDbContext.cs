using EcoFleetLogistics.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace EcoFleetLogistics.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Shipment> Shipments => Set<Shipment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TrackingNumber)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.HasIndex(e => e.TrackingNumber)
                    .IsUnique();

                entity.Property(e => e.ReceiverName)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.SenderName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DestinationAddress)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Weight)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .IsRequired();
            });
        }
    }
}