using EcoFleet.Shared.Kernel.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EcoFleet.Identity.Infrastructure.Persistence;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../EcoFleet.Identity.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection") ;

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        
        optionsBuilder.UseSqlServer(connectionString);

        return new IdentityDbContext(optionsBuilder.Options, new DesignTimeCurrentUserService());
    }

    private class DesignTimeCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => Guid.Empty;
        public Guid? TenantId => null;
        public string? UserEmail => null;
        public string? Role => "System";
        public bool IsAuthenticated => false;
    }
}