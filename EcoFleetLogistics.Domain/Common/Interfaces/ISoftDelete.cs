namespace EcoFleetLogistics.Domain.Common.Interfaces
{
    public interface ISoftDelete
    {
        bool IsDeleted {get;}
        DateTime? DeletedAt {get;}
        Guid? DeletedById {get;}
    }
}