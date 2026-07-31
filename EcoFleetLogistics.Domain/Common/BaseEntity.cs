using EcoFleetLogistics.Domain.Common.Interfaces;

namespace EcoFleetLogistics.Domain.Common
{
    public abstract class BaseEntity : IAuditableEntity, ISoftDelete
    {
        public Guid Id {get; protected set;}
        
        //Soft Delete Props
        public bool IsDeleted {get; protected set;}
        public DateTime? DeletedAt {get; protected set;}
        public Guid? DeletedById {get; protected set;}

        //Audit Props
        public DateTime CreatedAt {get; protected set;}
        public Guid? CreatedById {get; protected set;}
        public DateTime? UpdatedAt {get; protected set;}
        public Guid? UpdatedById {get; protected set;}

    }
}