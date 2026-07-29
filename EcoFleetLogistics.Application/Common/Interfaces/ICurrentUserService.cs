namespace EcoFleetLogistics.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        /// <summary>
        /// The ID of the user making the request at that moment (returns null if there is no token)
        /// </summary>
        Guid? UserId { get; }
    
        /// <summary>
        /// The e-mail address of the user sending the request at that moment
        /// </summary>
        string? UserEmail { get; }
    
        /// <summary>
        /// The Role of the user sending the request at that moment(Admin, FleetManager, Driver vb.)
        /// </summary>
        string? Role { get; }
    
        /// <summary>
        /// The ID of the company to which the user making the request is affiliated (for multi-tenancy)
        /// </summary>
        Guid? CompanyId { get; }
    
        /// <summary>
        /// Whether the user has logged in with a valid token
        /// </summary>
        bool IsAuthenticated { get; }
    }
}