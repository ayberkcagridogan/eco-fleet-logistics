using EcoFleet.Shared.Kernel.Primitives.Interfaces;

namespace EcoFleet.Shared.Kernel.Primitives
{
    public abstract class BaseEntity<TId> : IAuditableEntity, ISoftDelete
    {
        public TId Id { get; protected set; } = default!;
        
        //Soft Delete Props
        public bool IsDeleted {get; protected set;}
        public DateTime? DeletedAt {get; protected set;}
        public Guid? DeletedById {get; protected set;}

        //Audit Props
        public DateTime CreatedAt {get; protected set;}
        public Guid? CreatedById {get; protected set;}
        public DateTime? UpdatedAt {get; protected set;}
        public Guid? UpdatedById {get; protected set;}

        protected BaseEntity(TId id) => Id = id;
        protected BaseEntity() { }

    }
}