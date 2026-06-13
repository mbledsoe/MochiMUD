using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public interface ICommandNotificationService
    {
        Task SendToPlayersAsync(
            IEnumerable<Player> players,
            string message,
            CancellationToken cancellationToken = default);

        Task SendToPlayersInRoomExceptAsync(
            Guid roomId,
            Player excludedPlayer,
            string message,
            CancellationToken cancellationToken = default);

        Task SendPlayerStatsUpdateAsync(
            Player player,
            CancellationToken cancellationToken = default);
    }
}
