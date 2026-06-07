using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class MoveHandler : ICommandHandler
    {
        private readonly ILogger<MoveHandler> logger;
        private readonly MoveService moveService;

        public MoveHandler(ILogger<MoveHandler> logger, MoveService moveService)
        {
            this.logger = logger;
            this.moveService = moveService;
        }

        public string CommandName => "move";

        public async Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling move command: {Command}", command);

            var direction = GetDirection(command);

            if (direction is null)
            {
                await client.SendMessageAsync("Move where?", cancellationToken);
                return;
            }

            await moveService.MoveAsync(direction.Value, client, player, cancellationToken);
        }

        private static Direction? GetDirection(string command)
        {
            var commandParts = command.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            if (commandParts.Length < 2)
            {
                return null;
            }

            return Enum.TryParse<Direction>(commandParts[1], true, out var direction)
                ? direction
                : null;
        }
    }
}
