namespace EcoFleet.Shared.Kernel.Primitives.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt {get;}
        Guid? CreatedById {get;}
        DateTime? UpdatedAt {get;}
        Guid? UpdatedById {get;}
    }
}