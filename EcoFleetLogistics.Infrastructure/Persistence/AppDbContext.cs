using EcoFleetLogistics.Domain.Authentication;
using EcoFleetLogistics.Domain.Companies;
using EcoFleetLogistics.Domain.Shipments;
using EcoFleetLogistics.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace EcoFleetLogistics.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Company> Companies => Set<Company>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}