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

        public Task ProcessAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            var commandHandler = GetCommandHandler(command);

            if (commandHandler is null)
            {
                return Task.CompletedTask;
            }

            return commandHandler.HandleAsync(command, client, player, cancellationToken);
        }

        public ICommandHandler? GetCommandHandler(string command)
        {
            var commandName = CommandTextParser.GetCommandName(command);

            if (commandName is null)
            {
                logger.LogWarning("Received an empty command.");
                return null;
            }

            if (commandHandlers.TryGetValue(commandName, out var commandHandler))
            {
                return commandHandler;
            }

            var matchingHandlers = commandHandlers
                .Where(handlerEntry => handlerEntry.Key.StartsWith(commandName, StringComparison.OrdinalIgnoreCase))
                .Select(handlerEntry => handlerEntry.Value)
                .ToList();

            if (matchingHandlers.Count == 1)
            {
                return matchingHandlers[0];
            }

            if (matchingHandlers.Count > 1)
            {
                logger.LogWarning("Command abbreviation is ambiguous: {CommandName}", commandName);
                return null;
            }

            logger.LogWarning("No command handler registered for command: {CommandName}", commandName);
            return null;
        }
    }
}
