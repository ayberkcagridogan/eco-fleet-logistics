using Microsoft.EntityFrameworkCore;
using EcoFleet.Shared.Kernel.Persistence;
using EcoFleet.Shared.Kernel.Services.Interfaces;

namespace EcoFleet.Company.Infrastructure.Persistence;

public class CompanyDbContext : BaseDbContext<CompanyDbContext>
{
    public CompanyDbContext(DbContextOptions<CompanyDbContext> options , ICurrentUserService currentUserService) 
        : base(options, currentUserService){ }
    public DbSet<Domain.Companies.Company> Companies => Set<Domain.Companies.Company>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Company");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompanyDbContext).Assembly);


        base.OnModelCreating(modelBuilder);
    }
}