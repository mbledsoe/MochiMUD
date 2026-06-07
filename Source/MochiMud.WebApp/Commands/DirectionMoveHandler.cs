using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class DirectionMoveHandler : ICommandHandler
    {
        private readonly Direction direction;
        private readonly ILogger<DirectionMoveHandler> logger;
        private readonly MoveService moveService;

        public DirectionMoveHandler(
            string commandName,
            Direction direction,
            ILogger<DirectionMoveHandler> logger,
            MoveService moveService)
        {
            CommandName = commandName;
            this.direction = direction;
            this.logger = logger;
            this.moveService = moveService;
        }

        public string CommandName { get; }

        public async Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling direction move command: {Command}", command);

            await moveService.MoveAsync(direction, client, player, cancellationToken);
        }
    }
}
