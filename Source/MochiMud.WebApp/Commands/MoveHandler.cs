using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class MoveHandler : CommandHandlerBase
    {
        private readonly ILogger<MoveHandler> logger;
        private readonly MoveService moveService;

        public MoveHandler(ILogger<MoveHandler> logger, MoveService moveService)
        {
            this.logger = logger;
            this.moveService = moveService;
        }

        public override string CommandName => "move";

        public override async Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling move command: {Command}", command);

            var direction = CommandTextParser.GetEnumArgument<Direction>(command);

            if (direction is null)
            {
                await client.SendMessageAsync("Move where?", cancellationToken);
                return;
            }

            await moveService.MoveAsync(direction.Value, client, player, cancellationToken);
        }
    }
}
