using Microsoft.AspNetCore.SignalR;
using MochiMud.WebApp.Commands;
using MochiMud.WebApp.Game;
using MochiMud.WebApp.Players;

namespace MochiMud.WebApp.Hubs
{
    public class MudHub : Hub
    {
        private readonly CommandExecutionQueue commandExecutionQueue;
        private readonly GameStateService gameStateService;
        private readonly IHubContext<MudHub> hubContext;
        private readonly ILogger<MudHub> logger;
        private readonly PlayerConnectionRegistry playerConnectionRegistry;

        public MudHub(
            CommandExecutionQueue commandExecutionQueue,
            GameStateService gameStateService,
            IHubContext<MudHub> hubContext,
            ILogger<MudHub> logger,
            PlayerConnectionRegistry playerConnectionRegistry)
        {
            this.commandExecutionQueue = commandExecutionQueue;
            this.gameStateService = gameStateService;
            this.hubContext = hubContext;
            this.logger = logger;
            this.playerConnectionRegistry = playerConnectionRegistry;
        }

        public override async Task OnConnectedAsync()
        {
            var client = new SignalRCommandClient(hubContext, Context.ConnectionId);
            var player = await gameStateService.AddPlayerToGameAsync(client, Context.ConnectionAborted);

            playerConnectionRegistry.AddOrUpdate(Context.ConnectionId, player);

            logger.LogInformation(
                "Associated player {PlayerName} with connection {ConnectionId}.",
                player.Name,
                Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (playerConnectionRegistry.TryRemovePlayer(Context.ConnectionId, out var player) && player is not null)
            {
                gameStateService.RemovePlayerFromGame(player);
            }

            logger.LogInformation("Removed player for connection {ConnectionId}.", Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendCommand(string command)
        {
            logger.LogInformation("Received command from {ConnectionId}: {Command}", Context.ConnectionId, command);

            if (!playerConnectionRegistry.TryGetPlayer(Context.ConnectionId, out var player) || player is null)
            {
                logger.LogWarning("No player found for connection {ConnectionId}.", Context.ConnectionId);
                return;
            }

            var client = new SignalRCommandClient(hubContext, Context.ConnectionId);

            await commandExecutionQueue.EnqueueAsync(new QueuedCommand(command, client, player), Context.ConnectionAborted);
        }
    }
}
