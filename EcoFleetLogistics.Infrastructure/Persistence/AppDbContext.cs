using System.Linq.Expressions;
using EcoFleetLogistics.Application.Common.Interfaces;
using EcoFleetLogistics.Domain.Authentication;
using EcoFleetLogistics.Domain.Common;
using EcoFleetLogistics.Domain.Companies;
using EcoFleetLogistics.Domain.Shipments;
using EcoFleetLogistics.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EcoFleetLogistics.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Company> Companies => Set<Company>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ICompanyResource).IsAssignableFrom(entityType.ClrType))
                {
                    // e => _currentUserService.CompanyId == null || e.CompanyId == _currentUserService.CompanyId
                    var parameter = Expression.Parameter(entityType.ClrType, "e");

                    // _currentUserService.CompanyId
                    var currentCompayIdProp = Expression.Property(
                        Expression.Constant(_currentUserService),
                        nameof(ICurrentUserService.CompanyId));
                    
                    // e.CompanyId
                    var entityCompanyIdProp = Expression.Property(parameter, nameof(ICompanyResource.CompanyId));

                    // _currentUserService.CompanyId == null
                    var isCompanyIdNull = Expression.Equal(
                        currentCompayIdProp,
                        Expression.Constant(null, typeof(Guid?)));

                    // e.CompanyId == _currentUserService.CompanyId
                    var isCompanyIdMatch = Expression.Equal(
                        entityCompanyIdProp,
                        Expression.Convert(currentCompayIdProp, typeof(Guid)));
                    
                    // (CompanyId == null) OR (e.CompanyId == currentCompanyId)
                    var body = Expression.OrElse(isCompanyIdNull, isCompanyIdMatch);
                    var lambda = Expression.Lambda(body, parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}