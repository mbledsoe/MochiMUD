using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class MoveHandler : ICommandHandler
    {
        private readonly ILogger<MoveHandler> logger;
        private readonly RoomPresenter roomPresenter;
        private readonly IWorldDataService worldDataService;

        public MoveHandler(ILogger<MoveHandler> logger, RoomPresenter roomPresenter, IWorldDataService worldDataService)
        {
            this.logger = logger;
            this.roomPresenter = roomPresenter;
            this.worldDataService = worldDataService;
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

            var currentRoom = await worldDataService.GetRoomAsync(player.CurrentRoomId, cancellationToken);

            if (currentRoom is null)
            {
                logger.LogWarning("Room not found: {RoomId}", player.CurrentRoomId);
                await client.SendMessageAsync("You cannot move from here.", cancellationToken);
                return;
            }

            var exit = currentRoom.Exits.FirstOrDefault(exit => exit.Direction == direction);

            if (exit is null)
            {
                await client.SendMessageAsync($"You cannot move {direction}.", cancellationToken);
                return;
            }

            var destinationRoom = await worldDataService.GetRoomAsync(exit.DestinationRoomId, cancellationToken);

            if (destinationRoom is null)
            {
                logger.LogError("Destination room not found: {RoomId}", exit.DestinationRoomId);
                await client.SendMessageAsync("You cannot move there.", cancellationToken);
                return;
            }

            player.CurrentRoomId = exit.DestinationRoomId;

            await roomPresenter.TrySendRoomAsync(player.CurrentRoomId, client, cancellationToken);
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
