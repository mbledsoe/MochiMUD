using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public interface ICommandClient
    {
        Task SendMessageAsync(string message, CancellationToken cancellationToken = default);

        Task SendRoomAsync(Room room, CancellationToken cancellationToken = default);
    }
}
