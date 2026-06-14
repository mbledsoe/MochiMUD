using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public interface ICommandClient
    {
        Task SendMessageAsync(string message, CancellationToken cancellationToken = default);

        Task SendBannerAsync(string banner, CancellationToken cancellationToken = default);

        Task SendRoomAsync(
            Room room,
            IReadOnlyCollection<Mob> mobs,
            IReadOnlyCollection<Player> players,
            string? exitsText = null,
            CancellationToken cancellationToken = default);
    }
}
