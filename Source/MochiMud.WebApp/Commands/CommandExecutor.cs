using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class CommandExecutor
    {
        private readonly CommandProcessor commandProcessor;

        public CommandExecutor(CommandProcessor commandProcessor)
        {
            this.commandProcessor = commandProcessor;
        }

        public async Task ExecuteAsync(
            string command,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken = default)
        {
            var commandHandler = commandProcessor.GetCommandHandler(command);

            if (commandHandler is null)
            {
                return;
            }

            if (player.IsInFight && !commandHandler.CanExecuteInFight)
            {
                await client.SendMessageAsync("You cannot do that while you are in a fight.", cancellationToken);
                return;
            }

            await commandHandler.HandleAsync(command, client, player, cancellationToken);
        }
    }
}
