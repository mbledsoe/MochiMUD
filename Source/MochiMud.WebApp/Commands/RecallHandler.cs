using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Commands
{
    public class RecallHandler : CommandHandlerBase
    {
        private readonly ILogger<RecallHandler> logger;
        private readonly RecallService recallService;

        public RecallHandler(
            ILogger<RecallHandler> logger,
            RecallService recallService)
        {
            this.logger = logger;
            this.recallService = recallService;
        }

        public override string CommandName => "recall";

        public override async Task HandleAsync(
            string command,
            ICommandClient client,
            Player player,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Handling recall command: {Command}", command);

            await recallService.RecallAsync(player, client, cancellationToken);
        }
    }
}
