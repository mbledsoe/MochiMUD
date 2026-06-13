namespace MochiMud.WebApp.Players
{
    public interface IPlayerStore
    {
        Task SaveAsync(Player player, CancellationToken cancellationToken = default);

        Task<PlayerData?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> TryReserveNameAsync(string name, Guid accountId, CancellationToken cancellationToken = default);
    }
}
