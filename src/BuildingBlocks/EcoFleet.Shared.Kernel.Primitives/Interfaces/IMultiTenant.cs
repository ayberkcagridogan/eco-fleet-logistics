namespace EcoFleet.Shared.Kernel.Primitives.Interfaces
{
    public interface IMultiTenant
    {
        /// <summary>
        /// Company/Tenant ID to which the record belongs
        /// </summary>
        Guid TenantId { get; } 
    }
}