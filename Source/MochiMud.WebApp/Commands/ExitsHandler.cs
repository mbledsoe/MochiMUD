using MochiMud.WebApp.Players;
using MochiMud.WebApp.World;

namespace MochiMud.WebApp.Commands
{
    public class ExitsHandler : CommandHandlerBase
    {
        private readonly ExitsPresenter exitsPresenter;
        private readonly ILogger<ExitsHandler> logger;
        private readonly IWorldDataService worldDataService;

        public ExitsHandler(
            ExitsPresenter exitsPresenter,
            ILogger<ExitsHandler> logger,
            IWorldDataService worldDataService)
        {
            this.exitsPresenter = exitsPresenter;
            this.logger = logger;
            this.worldDataService = worldDataService;
        }

        public override bool CanExecuteInFight => true;

        public override string CommandName => "exits";

        public override async Task HandleAsync(
            string command,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling exits command: {Command}", command);

            var room = await worldDataService.GetRoomAsync(player.CurrentRoomId, cancellationToken);

            if (room is null)
            {
                await client.SendMessageAsync("You see no obvious exits.", cancellationToken);
                return;
            }

            await client.SendMessageAsync(await exitsPresenter.FormatExitsAsync(room, cancellationToken), cancellationToken);
        }
    }
}
