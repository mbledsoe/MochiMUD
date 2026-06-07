using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class LookHandler : CommandHandlerBase
    {
        private readonly ILogger<LookHandler> logger;
        private readonly RoomPresenter roomPresenter;

        public LookHandler(ILogger<LookHandler> logger, RoomPresenter roomPresenter)
        {
            this.logger = logger;
            this.roomPresenter = roomPresenter;
        }

        public override string CommandName => "look";

        public override bool CanExecuteInFight => true;

        public override async Task HandleAsync(string command, ICommandClient client, Player player, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling look command: {Command}", command);

            var roomWasSent = await roomPresenter.TrySendRoomAsync(player.CurrentRoomId, client, player, cancellationToken);

            if (!roomWasSent)
            {
                await client.SendMessageAsync("You see nothing special.", cancellationToken);
            }
        }
    }
}
