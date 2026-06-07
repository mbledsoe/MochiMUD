using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class RoomPresenter
    {
        private readonly ILogger<RoomPresenter> logger;
        private readonly IMobDataService mobDataService;
        private readonly IPlayerDataService playerDataService;
        private readonly IWorldDataService worldDataService;

        public RoomPresenter(
            ILogger<RoomPresenter> logger,
            IMobDataService mobDataService,
            IPlayerDataService playerDataService,
            IWorldDataService worldDataService)
        {
            this.logger = logger;
            this.mobDataService = mobDataService;
            this.playerDataService = playerDataService;
            this.worldDataService = worldDataService;
        }

        public async Task<bool> TrySendRoomAsync(
            Guid roomId,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken = default)
        {
            var room = await worldDataService.GetRoomAsync(roomId, cancellationToken);

            if (room is null)
            {
                logger.LogWarning("Room not found: {RoomId}", roomId);
                return false;
            }

            var mobs = await mobDataService.GetMobsInRoomAsync(room.Id, cancellationToken);
            var players = playerDataService
                .GetPlayersInRoom(room.Id)
                .Where(roomPlayer => !ReferenceEquals(roomPlayer, player))
                .ToArray();

            await client.SendRoomAsync(room, mobs, players, cancellationToken);

            return true;
        }
    }
}
