namespace MochiMud.WebApp.Mobs
{
    public interface IMobDataService
    {
        Task<IReadOnlyCollection<Mob>> GetMobsInRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
    }
}
