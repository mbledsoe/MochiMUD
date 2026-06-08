using Microsoft.Extensions.DependencyInjection;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class HelpHandler : CommandHandlerBase
    {
        private readonly ILogger<HelpHandler> logger;
        private readonly IServiceProvider serviceProvider;

        public HelpHandler(ILogger<HelpHandler> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public override string CommandName => "help";

        public override bool CanExecuteInFight => true;

        public override bool CanExecuteWhenDead => true;

        public override async Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling help command: {Command}", command);

            var commandNames = serviceProvider
                .GetServices<ICommandHandler>()
                .Select(commandHandler => commandHandler.CommandName)
                .Where(commandName => !string.Equals(commandName, CommandName, StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(commandName => commandName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            await client.SendMessageAsync($"Available commands: {string.Join(", ", commandNames)}", cancellationToken);
        }
    }
}
