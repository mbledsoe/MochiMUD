using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Game
{
    public class GameStateService
    {
        private const string FakePlayerNamePrefix = "Test Player";
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

        public async Task<Player> AddPlayerToGameAsync(ICommandClient client, CancellationToken cancellationToken = default)
        {
            var player = new Player($"{FakePlayerNamePrefix} {Random.Shared.Next(0, 10000):0000}");

            playerDataService.Add(player);

            logger.LogInformation("Added player {PlayerName} to the game.", player.Name);

            await commandExecutionQueue.EnqueueAsync(new QueuedCommand(InitialCommand, client, player), cancellationToken);

            return player;
        }

        public void RemovePlayerFromGame(Player player)
        {
            playerGroupService.RemovePlayer(player);
            playerDataService.Remove(player);

            logger.LogInformation("Removed player {PlayerName} from the game.", player.Name);
        }
    }
}
