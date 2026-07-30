namespace EcoFleetLogistics.Domain.Common
{
    public interface ICompanyResource
    {
        /// <summary>
        /// Company/Tenant ID to which the record belongs
        /// </summary>
        Guid CompanyId { get; } 
    }
}