using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class LookHandler : ICommandHandler
    {
        private readonly ILogger<LookHandler> logger;
        private readonly IWorldDataService worldDataService;

        public LookHandler(ILogger<LookHandler> logger, IWorldDataService worldDataService)
        {
            this.logger = logger;
            this.worldDataService = worldDataService;
        }

        public string CommandName => "look";

        public async Task HandleAsync(string command, ICommandClient client, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling look command: {Command}", command);

            var room = await worldDataService.GetRoomAsync(WorldConstants.DefaultStartRoomId, cancellationToken);

            if (room is null)
            {
                logger.LogWarning("Room not found: {RoomId}", WorldConstants.DefaultStartRoomId);
                await client.SendMessageAsync("You see nothing special.", cancellationToken);
                return;
            }

            await client.SendRoomAsync(room, cancellationToken);
        }
    }
}
