using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class CommandProcessor
    {
        private readonly IReadOnlyDictionary<string, ICommandHandler> commandHandlers;
        private readonly ILogger<CommandProcessor> logger;

        public CommandProcessor(IEnumerable<ICommandHandler> commandHandlers, ILogger<CommandProcessor> logger)
        {

            this.commandHandlers = commandHandlers.ToDictionary(
                commandHandler => commandHandler.CommandName,
                StringComparer.OrdinalIgnoreCase);
            this.logger = logger;
        }

        public async Task ProcessAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            var commandName = GetCommandName(command);

            if (commandName is null)
            {
                logger.LogWarning("Received an empty command.");
                return;
            }

            if (!commandHandlers.TryGetValue(commandName, out var commandHandler))
            {
                logger.LogWarning("No command handler registered for command: {CommandName}", commandName);
                return;
            }

            await commandHandler.HandleAsync(command, client, player, cancellationToken);
        }

        private static string? GetCommandName(string command)
        {
            var trimmedCommand = command.AsSpan().Trim();

            if (trimmedCommand.IsEmpty)
            {
                return null;
            }

            for (var index = 0; index < trimmedCommand.Length; index++)
            {
                if (char.IsWhiteSpace(trimmedCommand[index]))
                {
                    return trimmedCommand[..index].ToString();
                }
            }

            return trimmedCommand.ToString();
        }
    }
}
