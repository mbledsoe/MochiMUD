using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public interface ICommandHandler
    {
        string CommandName { get; }

        Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default);
    }
}
