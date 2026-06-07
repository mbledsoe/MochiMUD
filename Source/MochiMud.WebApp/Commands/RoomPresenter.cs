using MochiMud.WebApp.Mobs;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class RoomPresenter
    {
        private readonly ILogger<RoomPresenter> logger;
        private readonly IMobDataService mobDataService;
        private readonly IWorldDataService worldDataService;

        public RoomPresenter(ILogger<RoomPresenter> logger, IMobDataService mobDataService, IWorldDataService worldDataService)
        {
            this.logger = logger;
            this.mobDataService = mobDataService;
            this.worldDataService = worldDataService;
        }

        public async Task<bool> TrySendRoomAsync(Guid roomId, ICommandClient client, CancellationToken cancellationToken = default)
        {
            var room = await worldDataService.GetRoomAsync(roomId, cancellationToken);

            if (room is null)
            {
                logger.LogWarning("Room not found: {RoomId}", roomId);
                return false;
            }

            var mobs = await mobDataService.GetMobsInRoomAsync(room.Id, cancellationToken);

            await client.SendRoomAsync(room, mobs, cancellationToken);

            return true;
        }
    }
}
