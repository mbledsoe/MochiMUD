using MochiMud.WebApp.Characters;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class MoveService
    {
        private readonly ICommandNotificationService commandNotificationService;
        private readonly ILogger<MoveService> logger;
        private readonly PlayerGroupService playerGroupService;
        private readonly RoomPresenter roomPresenter;
        private readonly IWorldDataService worldDataService;

        public MoveService(
            ICommandNotificationService commandNotificationService,
            ILogger<MoveService> logger,
            PlayerGroupService playerGroupService,
            RoomPresenter roomPresenter,
            IWorldDataService worldDataService)
        {
            this.commandNotificationService = commandNotificationService;
            this.logger = logger;
            this.playerGroupService = playerGroupService;
            this.roomPresenter = roomPresenter;
            this.worldDataService = worldDataService;
        }

        public async Task MoveAsync(
            Direction direction,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken = default)
        {
            if (!CanMove(player))
            {
                await client.SendMessageAsync("You cannot move right now.", cancellationToken);
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

            var followers = playerGroupService
                .GetFollowers(player)
                .Where(follower => follower.CurrentRoomId == currentRoom.Id && CanMove(follower))
                .ToArray();

            player.CurrentRoomId = exit.DestinationRoomId;

            foreach (var follower in followers)
            {
                follower.CurrentRoomId = exit.DestinationRoomId;
            }

            await commandNotificationService.SendToPlayersInRoomExceptAsync(
                currentRoom.Id,
                player,
                $"{player.Name} left {direction}.",
                cancellationToken);

            await commandNotificationService.SendToPlayersAsync(
                followers,
                $"You follow {player.Name} {direction}.",
                cancellationToken);

            await roomPresenter.TrySendRoomAsync(player.CurrentRoomId, client, player, cancellationToken);
        }

        private static bool CanMove(Player player)
        {
            return player.State == CharacterState.Standing;
        }
    }
}
