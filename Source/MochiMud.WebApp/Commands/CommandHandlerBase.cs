using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        public virtual bool CanExecuteInFight => false;

        public abstract string CommandName { get; }

        public abstract Task HandleAsync(
            string command,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken = default);
    }
}
