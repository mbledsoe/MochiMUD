using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Game
{
    public class GameStateService
    {
        private const string InitialCommand = "look";

        private readonly CommandExecutionQueue commandExecutionQueue;
        private readonly ILogger<GameStateService> logger;
        private readonly IPlayerDataService playerDataService;
        private readonly PlayerGroupService playerGroupService;

        public GameStateService(
            CommandExecutionQueue commandExecutionQueue,
            ILogger<GameStateService> logger,
            IPlayerDataService playerDataService,
            PlayerGroupService playerGroupService)
        {
            this.commandExecutionQueue = commandExecutionQueue;
            this.logger = logger;
            this.playerDataService = playerDataService;
            this.playerGroupService = playerGroupService;
        }

        public async Task AddPlayerToGameAsync(Player player, ICommandClient client, CancellationToken cancellationToken = default)
        {
            playerDataService.Add(player);

            logger.LogInformation("Added player {PlayerName} to the game.", player.Name);

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
