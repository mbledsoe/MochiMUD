using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Game
{
    public class GameStateService
    {
        private const string InitialCommand = "look";

        private readonly CommandExecutionQueue commandExecutionQueue;
        private readonly ICommandNotificationService commandNotificationService;
        private readonly ILogger<GameStateService> logger;
        private readonly IPlayerDataService playerDataService;
        private readonly PlayerGroupService playerGroupService;
        private readonly WelcomeBannerProvider welcomeBannerProvider;

        public GameStateService(
            CommandExecutionQueue commandExecutionQueue,
            ICommandNotificationService commandNotificationService,
            ILogger<GameStateService> logger,
            IPlayerDataService playerDataService,
            PlayerGroupService playerGroupService,
            WelcomeBannerProvider welcomeBannerProvider)
        {
            this.commandExecutionQueue = commandExecutionQueue;
            this.commandNotificationService = commandNotificationService;
            this.logger = logger;
            this.playerDataService = playerDataService;
            this.playerGroupService = playerGroupService;
            this.welcomeBannerProvider = welcomeBannerProvider;
        }

        public async Task AddPlayerToGameAsync(Player player, ICommandClient client, CancellationToken cancellationToken = default)
        {
            playerDataService.Add(player);

            logger.LogInformation("Added player {PlayerName} to the game.", player.Name);

            var banner = welcomeBannerProvider.GetBanner();

            if (banner is not null)
            {
                await client.SendBannerAsync(banner, cancellationToken);
            }

            await commandNotificationService.SendPlayerStatsUpdateAsync(player, cancellationToken);

            await commandExecutionQueue.EnqueueAsync(new QueuedCommand(InitialCommand, client, player), cancellationToken);
        }

        public void RemovePlayerFromGame(Player player)
        {
            playerGroupService.RemovePlayer(player);
            playerDataService.Remove(player);

            logger.LogInformation("Removed player {PlayerName} from the game.", player.Name);
        }
    }
}
