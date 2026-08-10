namespace EcoFleet.Shared.Kernel.Primitives.Interfaces
{
    public interface ISoftDelete
    {
        bool IsDeleted {get;}
        DateTime? DeletedAt {get;}
        Guid? DeletedById {get;}
    }
}