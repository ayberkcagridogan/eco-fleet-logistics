using EcoFleet.Identity.Domain.Authentication;
using EcoFleet.Identity.Domain.Users;
using EcoFleet.Shared.Kernel.Persistence;
using EcoFleet.Shared.Kernel.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoFleet.Identity.Infrastructure.Persistence;

public class IdentityDbContext : BaseDbContext<IdentityDbContext>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, ICurrentUserService currentUserService) 
        : base(options, currentUserService){ }

    public DbSet<User> Users => Set<User>();
   public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}