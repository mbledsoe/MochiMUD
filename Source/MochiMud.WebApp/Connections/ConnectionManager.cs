using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Game;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Connections
{
    public class ConnectionManager
    {
        private readonly CommandExecutionQueue commandExecutionQueue;
        private readonly GameStateService gameStateService;
        private readonly ILogger<ConnectionManager> logger;
        private readonly IPlayerStore playerStore;
        private readonly PlayerConnectionRegistry playerConnectionRegistry;
        private readonly PlayerCreationService playerCreationService;

        public ConnectionManager(
            CommandExecutionQueue commandExecutionQueue,
            GameStateService gameStateService,
            ILogger<ConnectionManager> logger,
            IPlayerStore playerStore,
            PlayerConnectionRegistry playerConnectionRegistry,
            PlayerCreationService playerCreationService)
        {
            this.commandExecutionQueue = commandExecutionQueue;
            this.gameStateService = gameStateService;
            this.logger = logger;
            this.playerStore = playerStore;
            this.playerConnectionRegistry = playerConnectionRegistry;
            this.playerCreationService = playerCreationService;
        }

        public async Task OnConnectedAsync(IClientConnection connection, CancellationToken cancellationToken = default)
        {
            var playerData = await playerStore.LoadAsync(connection.AccountId, cancellationToken);

            if (playerData is null)
            {
                await playerCreationService.BeginAsync(connection, cancellationToken);
                return;
            }

            var player = Player.FromData(playerData);

            playerConnectionRegistry.AddOrUpdate(connection.ConnectionId, player);

            await gameStateService.AddPlayerToGameAsync(player, connection.Client, cancellationToken);

            logger.LogInformation(
                "Associated player {PlayerName} with connection {ConnectionId}.",
                player.Name,
                connection.ConnectionId);
        }

        public async Task OnInputAsync(IClientConnection connection, string input, CancellationToken cancellationToken = default)
        {
            if (playerCreationService.IsInCreation(connection.ConnectionId))
            {
                await playerCreationService.HandleInputAsync(connection, input, cancellationToken);
                return;
            }

            if (!playerConnectionRegistry.TryGetPlayer(connection.ConnectionId, out var player) || player is null)
            {
                logger.LogWarning("No player found for connection {ConnectionId}.", connection.ConnectionId);
                return;
            }

            await commandExecutionQueue.EnqueueAsync(new QueuedCommand(input, connection.Client, player), cancellationToken);
        }

        public Task OnDisconnectedAsync(IClientConnection connection, CancellationToken cancellationToken = default)
        {
            playerCreationService.Abandon(connection.ConnectionId);

            if (playerConnectionRegistry.TryRemovePlayer(connection.ConnectionId, out var player) && player is not null)
            {
                gameStateService.RemovePlayerFromGame(player);
            }

            logger.LogInformation("Removed player for connection {ConnectionId}.", connection.ConnectionId);

            return Task.CompletedTask;
        }
    }
}
