using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class RecallHandler : CommandHandlerBase
    {
        private readonly ICommandNotificationService commandNotificationService;
        private readonly ILogger<RecallHandler> logger;
        private readonly RoomPresenter roomPresenter;

        public RecallHandler(
            ICommandNotificationService commandNotificationService,
            ILogger<RecallHandler> logger,
            RoomPresenter roomPresenter)
        {
            this.commandNotificationService = commandNotificationService;
            this.logger = logger;
            this.roomPresenter = roomPresenter;
        }

        public override string CommandName => "recall";

        public override async Task HandleAsync(
            string command,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling recall command: {Command}", command);

            player.CurrentRoomId = HockesdenWorldBuilder.TownSquareRoomId;

            await commandNotificationService.SendToPlayersInRoomExceptAsync(
                player.CurrentRoomId,
                player,
                $"A shimmering cloud suddenly appears and {player.Name} steps through!",
                cancellationToken);

            await client.SendMessageAsync("You recall to Hockesden Town Square.", cancellationToken);
            await roomPresenter.TrySendRoomAsync(player.CurrentRoomId, client, player, cancellationToken);
        }
    }
}
