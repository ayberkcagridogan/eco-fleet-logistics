using System.Dynamic;

namespace EcoFleetLogistics.Domain.Common.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt {get;}
        Guid? CreatedById {get;}
        DateTime? UpdatedAt {get;}
        Guid? UpdatedById {get;}
    }
}