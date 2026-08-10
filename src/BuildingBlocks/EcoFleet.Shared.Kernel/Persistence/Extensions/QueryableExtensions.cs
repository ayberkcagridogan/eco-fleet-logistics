using Microsoft.EntityFrameworkCore;

namespace EcoFleet.Shared.Kernel.Persistence.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Conditionally bypasses the Global Query Filter (e.g., only if Admin).
        /// </summary>
        public static IQueryable<TEntity> IgnoreTenantFilterIf<TEntity>(
            this IQueryable<TEntity> query, 
            bool condition) where TEntity : class
        {
            return condition ? query.IgnoreQueryFilters() : query;
        }
    }
}