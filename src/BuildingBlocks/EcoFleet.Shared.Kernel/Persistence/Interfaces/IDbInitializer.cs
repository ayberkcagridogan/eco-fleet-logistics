namespace EcoFleet.Shared.Kernel.Persistence.Interfaces
{
    public interface IDbInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}