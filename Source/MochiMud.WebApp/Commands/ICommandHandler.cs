using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public interface ICommandHandler
    {
        bool CanExecuteInFight { get; }

        bool CanExecuteWhenDead { get; }

        string CommandName { get; }

        Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default);
    }
}
