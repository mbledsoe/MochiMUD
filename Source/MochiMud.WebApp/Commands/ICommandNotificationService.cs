using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public interface ICommandNotificationService
    {
        Task SendToPlayersInRoomExceptAsync(
            Guid roomId,
            Player excludedPlayer,
            string message,
            CancellationToken cancellationToken = default);
    }
}
