using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class LookHandler : CommandHandlerBase
    {
        private readonly ILogger<LookHandler> logger;
        private readonly RoomPresenter roomPresenter;
        private readonly IWorldDataService worldDataService;

        public LookHandler(
            ILogger<LookHandler> logger,
            RoomPresenter roomPresenter,
            IWorldDataService worldDataService)
        {
            this.logger = logger;
            this.roomPresenter = roomPresenter;
            this.worldDataService = worldDataService;
        }

        public override string CommandName => "look";

        public override bool CanExecuteInFight => true;

        public override async Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling look command: {Command}", command);

            var targetName = CommandTextParser.GetArguments(command);

            if (targetName is not null)
            {
                await LookAtTargetAsync(targetName, client, player, cancellationToken);
                return;
            }

            var roomWasSent = await roomPresenter.TrySendRoomAsync(player.CurrentRoomId, client, player, cancellationToken);

            if (!roomWasSent)
            {
                await client.SendMessageAsync("You see nothing special.", cancellationToken);
            }
        }

        private async Task LookAtTargetAsync(
            string targetName,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken)
        {
            var room = await worldDataService.GetRoomAsync(player.CurrentRoomId, cancellationToken);

            if (room is null)
            {
                await client.SendMessageAsync("You don't see that here.", cancellationToken);
                return;
            }

            var roomObject = room.Objects.FirstOrDefault(roomObject => IsMatch(roomObject, targetName));

            if (roomObject is null)
            {
                await client.SendMessageAsync("You don't see that here.", cancellationToken);
                return;
            }

            await client.SendMessageAsync(roomObject.ReadText ?? roomObject.Description, cancellationToken);
        }

        private static bool IsMatch(RoomObject roomObject, string targetName)
        {
            var normalizedTarget = targetName.Trim();

            if (normalizedTarget.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
            {
                normalizedTarget = normalizedTarget[4..].Trim();
            }

            return string.Equals(roomObject.Name, normalizedTarget, StringComparison.OrdinalIgnoreCase);
        }
    }
}
