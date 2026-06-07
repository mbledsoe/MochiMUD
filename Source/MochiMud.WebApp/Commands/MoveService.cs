using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class MoveService
    {
        private readonly ICommandNotificationService commandNotificationService;
        private readonly ILogger<MoveService> logger;
        private readonly RoomPresenter roomPresenter;
        private readonly IWorldDataService worldDataService;

        public MoveService(
            ICommandNotificationService commandNotificationService,
            ILogger<MoveService> logger,
            RoomPresenter roomPresenter,
            IWorldDataService worldDataService)
        {
            this.commandNotificationService = commandNotificationService;
            this.logger = logger;
            this.roomPresenter = roomPresenter;
            this.worldDataService = worldDataService;
        }

        public async Task MoveAsync(
            Direction direction,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken = default)
        {
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

            await commandNotificationService.SendToPlayersInRoomExceptAsync(
                currentRoom.Id,
                player,
                $"{player.Name} left {direction}.",
                cancellationToken);

            player.CurrentRoomId = exit.DestinationRoomId;

            await roomPresenter.TrySendRoomAsync(player.CurrentRoomId, client, player, cancellationToken);
        }
    }
}
