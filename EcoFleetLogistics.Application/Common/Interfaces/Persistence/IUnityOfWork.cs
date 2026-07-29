namespace EcoFleetLogistics.Application.Common.Interfaces.Persistence
{
    public interface IUnityOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}